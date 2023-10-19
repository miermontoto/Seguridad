using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace UsarCertificado
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] Msg = new byte[64];

            for(int i = 0; i < Msg.Length; i++)
            {
                Msg[i] = (byte) i;
            }

            // Fase 1. Acceso a un certificado del almacén de certificados
            string NombreCert = "MIER MONTOTO, JUAN FRANCISCO (AUTENTICACIÓN)";
            X509Certificate2 Cert = ExtraeCertificado(NombreCert, StoreName.My, StoreLocation.CurrentUser);

            // Fase 2. Uso del certificado
            Console.WriteLine();
            Console.WriteLine("--- CERTIFICADO ---");
            Console.WriteLine("Subject: " + Cert.Subject);
            Console.WriteLine("HasPrivateKey: " + Cert.HasPrivateKey);
            Console.WriteLine();

            if (Cert.HasPrivateKey)
            {
                Console.WriteLine("--- CLAVE PRIVADA ---");
                Console.WriteLine("Algoritmo: " + Cert.PrivateKey.KeyExchangeAlgorithm);
                Console.WriteLine("Longitud: " + Cert.PrivateKey.KeySize);
                Console.WriteLine();
            }

            RSACryptoServiceProvider ProvRSA1 = (RSACryptoServiceProvider) Cert.PublicKey.Key;
            //VerParam(ProvRSA1, false);

            var ProvRSA2 = (RSACryptoServiceProvider) Cert.PrivateKey;
            //VerParam(ProvRSA2, true);

            if (ProvRSA2 == null)
            {
                Console.WriteLine("ERROR: No se ha podido acceder a la clave privada");
                Environment.Exit(1);
            }

            Console.WriteLine("Se ha creado un proveedor con clave completa.");

            // Cifrar con la clave pública
            byte[] Cifrado = ProvRSA1.Encrypt(Msg, false);
            Console.WriteLine("Cifrado con clave pública: " + BitConverter.ToString(Cifrado));

            // Descifrar con la clave privada
            byte[] Descifrado = ProvRSA2.Decrypt(Cifrado, false);
            Console.WriteLine("Descifrado con clave privada: " + BitConverter.ToString(Descifrado));
            
            // Firmar con la clave privada
            byte[] Firma = ProvRSA2.SignData(Msg, CryptoConfig.MapNameToOID("SHA1"));
            Console.WriteLine("Firma con clave privada: " + BitConverter.ToString(Firma));

            // Verificar con la clave pública
            bool Verifica;

            Msg[0] = 0xFF;
            Verifica = ProvRSA1.VerifyData(Msg, CryptoConfig.MapNameToOID("SHA1"), Firma);
            Console.WriteLine("Verifica con mensaje estropeado: " + Verifica);

            Msg[0] = 0x00;
            byte FirmaAux = Firma[0];
            Firma[0] = 0xFF;
            Verifica = ProvRSA1.VerifyData(Msg, CryptoConfig.MapNameToOID("SHA1"), Firma);
            Console.WriteLine("Verifica con firma estropeada: " + Verifica);

            Firma[0] = FirmaAux;
            Verifica = ProvRSA1.VerifyData(Msg, CryptoConfig.MapNameToOID("SHA1"), Firma);
            Console.WriteLine("Verifica con todo correcto: " + Verifica);
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

            foreach (X509Certificate2 Cert in ColeCert)
            {
                Console.WriteLine("Subject: " + Cert.Subject);
            }
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

        internal static void VerParam(RSACryptoServiceProvider ProRSA, bool ExportaPriv)
        {
            RSAParameters ParamRSA = new RSAParameters();
            try
            {
                ParamRSA = ProRSA.ExportParameters(ExportaPriv);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPCION: " + e);
                Environment.Exit(1);
            }

            Console.WriteLine("\n--INICIO CLAVE RSA---------------------------");

            Console.WriteLine("Modulo (n) {0} bytes", ParamRSA.Modulus.Length);
            for (int i = 0; i < ParamRSA.Modulus.Length; i++)
                Console.Write("{0,2:X} ", ParamRSA.Modulus[i]);

            Console.WriteLine("\nExponente Publico (e) {0} bytes", ParamRSA.Exponent.Length);
            for (int i = 0; i < ParamRSA.Exponent.Length; i++)
                Console.Write("{0,2:X} ", ParamRSA.Exponent[i]);

            if (ExportaPriv)
            {
                Console.WriteLine("\nExponente Privado (d) {0} bytes", ParamRSA.D.Length);
                for (int i = 0; i < ParamRSA.D.Length; i++)
                    Console.Write("{0,2:X} ", ParamRSA.D[i]);
            }

            Console.WriteLine("\n--FIN CLAVE RSA------------------------------");
        }
    }
}
