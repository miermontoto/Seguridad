using Apoyo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ObtenerCertificado
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var Ayuda = new Ayuda();
            var Msg = new byte[64];

            for(int i = 0; i < Msg.Length; i++)
            {
                Msg[i] = (byte)i;
            }

            var CertCliente = ExtraeCertificado("zmCLI.as", StoreName.My, StoreLocation.CurrentUser);

            byte[] MsgCmsFirmadoCod = FirmaCMS(Msg, CertCliente, false);

            Console.WriteLine(VerificaCMS(Msg, MsgCmsFirmadoCod, false));

            byte[] MsgCmsCifradoCod = CifraCMS(Msg, CertCliente);
            Ayuda.WriteHex(MsgCmsCifradoCod, MsgCmsCifradoCod.Length);

            byte[] MsgCmsDescifrado = DescifraCMS(MsgCmsCifradoCod);
            Ayuda.WriteHex(MsgCmsDescifrado, MsgCmsDescifrado.Length);

            // 7. Anidación
            // --- EMISOR ---
            byte[] MsgCmsFirmadoCod3 = FirmaCMS(Msg, CertCliente, false);
            byte[] MsgCmsCifradoCod3 = CifraCMS(MsgCmsFirmadoCod3, CertCliente);

            // --- RECEPTOR ---
            byte[] MsgCmsDescifrado3 = DescifraCMS(MsgCmsCifradoCod3);
            bool Verifica2 = VerificaCMS(MsgCmsDescifrado3, MsgCmsFirmadoCod3, false);

            Console.WriteLine("Verifica2: " + Verifica2);
        }

        internal static byte[] DescifraCMS(byte[] CmsCifradoCodificado)
        {
            EnvelopedCms CmsCifrado = new EnvelopedCms();
            CmsCifrado.Decode(CmsCifradoCodificado);

            RecipientInfoCollection Recipientes = CmsCifrado.RecipientInfos;

            if(Recipientes.Count != 1)
            {
                throw new CryptographicException("Número de recipientes incorrecto");
            }

            RecipientInfo Receptor = Recipientes[0];
            Console.WriteLine("Tipo del receptor: " + Receptor.RecipientIdentifier.Type);   
            Console.WriteLine("Valor del identificador del receptor: " + Receptor.RecipientIdentifier.Value);

            CmsCifrado.Decrypt(Receptor);

            return CmsCifrado.ContentInfo.Content;
        }

        internal static byte[] FirmaCMS(byte[] Msg, X509Certificate2 CertFirma, bool Desasociada)
        {
            Oid IdMsg = new Oid("1.2.840.113549.1.7.1");
            ContentInfo CI = new ContentInfo(IdMsg, Msg);
            SignedCms CmsFirmado = new SignedCms(SubjectIdentifierType.IssuerAndSerialNumber, CI, Desasociada);
            CmsSigner FirmanteCMS = new CmsSigner(CertFirma);

            Console.WriteLine("Certificate.Subject: " + FirmanteCMS.Certificate.Subject);

            CmsFirmado.ComputeSignature(FirmanteCMS);

            return CmsFirmado.Encode();
        }

        internal static byte[] CifraCMS(byte[] Msg, X509Certificate2 CertReceptor)
        {
            ContentInfo CI = new ContentInfo(Msg);
            EnvelopedCms CmsCifrado = new EnvelopedCms(CI);
            CmsCifrado.ContentEncryptionAlgorithm.Oid.Value = "2.16.840.1.101.3.4.1.42";

            Console.WriteLine("Algoritmo: " + CmsCifrado.ContentEncryptionAlgorithm.Oid.FriendlyName);
            Console.WriteLine("Oid: " + CmsCifrado.ContentEncryptionAlgorithm.Oid.Value);
            Console.WriteLine("Longitud de la clave: " + CmsCifrado.ContentEncryptionAlgorithm.KeyLength);

            CmsRecipient ReceptorCMS = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, CertReceptor);
            CmsCifrado.Encrypt(ReceptorCMS);
            return CmsCifrado.Encode();
        }

        internal static bool VerificaCMS(byte[] Msg, byte[] CmsFirmadoCodificado, bool Desasociada)
        {
            SignedCms CmsFirmado;

            if(Desasociada)
            {
                Oid IdMsg = new Oid("1.2.840.113549.1.7.1");
                ContentInfo CI = new ContentInfo(IdMsg, Msg);
                CmsFirmado = new SignedCms(SubjectIdentifierType.IssuerAndSerialNumber, CI, Desasociada);
            } 
            else
            {
                CmsFirmado = new SignedCms();
            }

            CmsFirmado.Decode(CmsFirmadoCodificado);
            bool Resultado;

            try
            {
                Console.WriteLine("Comprobando firma...");
                CmsFirmado.CheckSignature(true);
                Resultado = true;
                Console.WriteLine("La firma es válida");
            } catch (CryptographicException e)
            {
                Resultado = false;
            }

            return Resultado;
        }

        internal static X509Certificate2 ExtraeCertificado(string Nombre, StoreName Name, StoreLocation Location)
        {
            // Fase 1. Abrir el almacén y extraer sus certificados
            X509Store Almacen = new X509Store(Name, Location);
            Console.WriteLine("Name: " + Almacen.Name);
            Console.WriteLine("Location: " + Almacen.Location);

            Almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection ColeCert = Almacen.Certificates;
            Almacen.Close();

            Console.WriteLine("Número de certificados: " + ColeCert.Count);

            // Fase 2. Buscar un certificado
            var CertsValidos = ColeCert.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            Console.WriteLine("Número de certificados válidos: " + CertsValidos.Count);
            var CertsExpirados = ColeCert.Find(X509FindType.FindByTimeExpired, DateTime.Now, false);
            Console.WriteLine("Número de certificados expirados: " + CertsExpirados.Count);

            var CertsEncontrados = ColeCert.Find(X509FindType.FindBySubjectName, Nombre, false);
            if (CertsEncontrados.Count == 0)
            {
                CertsEncontrados = ColeCert.Find(X509FindType.FindBySubjectDistinguishedName, Nombre, false);
            }

            Console.WriteLine("Número de certificados encontrados: " + CertsEncontrados.Count);
            if (CertsEncontrados.Count > 1)
            {
                Console.WriteLine("Se ha encontrado más de un certificado. Devolviendo el primero...");
                return CertsEncontrados[0];
            }

            if (CertsEncontrados.Count == 0)
            {
                Console.WriteLine("No se ha encontrado ningún certificado");
                return null;
            }

            return CertsEncontrados[0];
        }
    }
}
