using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.Pkcs;
using Apoyo;
using System.IO;

namespace FIRMA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Declaraciones iniciales de variables
            Ayuda ayuda = new Ayuda();
            string NombreFichero = "Fichero.dat";
            string NombreFicheroFirma = "Fichero.p7b";
            string NombreFicheroCifra = "FCifrado.dat";
            int Longitud = (int) ayuda.BytesFichero(NombreFichero);
            byte[] Msg = new byte[Longitud];
            bool Desasociada = false;

            // Lectura del fichero a cifrar
            FileStream FsLectura = new FileStream(NombreFichero, FileMode.Open);
            BinaryReader Reader = new BinaryReader(FsLectura);
            Reader.Read(Msg, 0, Longitud);
            Reader.Close();
            FsLectura.Close();

            // Extracción de los certificados cargados
            X509Certificate2 CertEmisor = ExtraeCertificado("zpemi23", StoreName.My, StoreLocation.CurrentUser);
            X509Certificate2 CertReceptor = ExtraeCertificado("zprec23", StoreName.My, StoreLocation.CurrentUser);

            // Firma y escritura del nuevo fichero
            byte[] Firma = FirmaCMS(Msg, CertEmisor, Desasociada);
            ayuda.GuardaBufer(NombreFicheroFirma, Firma);

            // Verificación de la firma tras la lectura del fichero
            byte[] FirmaRecuperada = new byte[Firma.Length];
            ayuda.CargaBufer(NombreFicheroFirma, FirmaRecuperada);
            VerificaCMS(Msg, FirmaRecuperada, Desasociada);

            // Cifrado y guardado del cifrado
            byte[] MsgCifrado = CifraCMS(Msg, CertReceptor);
            ayuda.GuardaBufer(NombreFicheroCifra, MsgCifrado);

            // Lectura y descifrado del mensaje
            byte[] MsgCifradoRecuperado = new byte[MsgCifrado.Length];
            ayuda.CargaBufer(NombreFicheroCifra, MsgCifradoRecuperado);
            byte[] MsgDescifrado = DescifraCMS(MsgCifradoRecuperado);
            Console.WriteLine("Contenido descifrado:");
            ayuda.WriteHex(MsgDescifrado, MsgDescifrado.Length);
        }

        internal static byte[] DescifraCMS(byte[] CmsCifradoCodificado)
        {
            EnvelopedCms CmsCifrado = new EnvelopedCms();
            CmsCifrado.Decode(CmsCifradoCodificado);

            RecipientInfoCollection Recipientes = CmsCifrado.RecipientInfos;

            if (Recipientes.Count != 1)
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
            Ayuda a = new Ayuda();
            Oid IdMsg = new Oid("1.2.840.113549.1.7.1");
            ContentInfo CI = new ContentInfo(IdMsg, Msg);
            SignedCms CmsFirmado = new SignedCms(SubjectIdentifierType.IssuerAndSerialNumber, CI, Desasociada);
            CmsSigner FirmanteCMS = new CmsSigner(CertFirma);

            Console.WriteLine("Certificate.Subject: " + FirmanteCMS.Certificate.Subject);
            Console.WriteLine("Contenido a cifrar: ");
            a.WriteHex(Msg, Msg.Length);

            CmsFirmado.ComputeSignature(FirmanteCMS);

            return CmsFirmado.Encode();
        }

        internal static byte[] CifraCMS(byte[] Msg, X509Certificate2 CertReceptor)
        {
            ContentInfo CI = new ContentInfo(Msg);
            EnvelopedCms CmsCifrado = new EnvelopedCms(CI);
            CmsCifrado.ContentEncryptionAlgorithm.Oid.Value = "2.16.840.1.101.3.4.1.42";

            Console.WriteLine("Algoritmo: " + CmsCifrado.ContentEncryptionAlgorithm.Oid.FriendlyName);
            Console.WriteLine("Certificate.Subject: " + new CmsSigner(CertReceptor).Certificate.Subject);

            CmsRecipient ReceptorCMS = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, CertReceptor);
            CmsCifrado.Encrypt(ReceptorCMS);
            return CmsCifrado.Encode();
        }

        internal static bool VerificaCMS(byte[] Msg, byte[] CmsFirmadoCodificado, bool Desasociada)
        {
            SignedCms CmsFirmado;

            if (Desasociada)
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
            }
            catch (CryptographicException e)
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
