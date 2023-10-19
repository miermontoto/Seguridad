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
            var CertAC = ExtraeCertificado("zmAC.as", StoreName.Root, StoreLocation.CurrentUser);

            byte[] MsgCmsFirmadoCod = FirmaCMS(Msg, CertCliente, false);

            // Guardar Msg en el fichero "Fichero.dat"
            Ayuda.GuardaBufer("Fichero.dat", Msg);

            // Guardar MsgCmsFirmadoCod en el fichero "Fichero.p7b"
            Ayuda.GuardaBufer("Fichero.p7b", MsgCmsFirmadoCod);


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
