using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace BuscarCertificados
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string NombreSujetoCer = "CN=zpac.as";

            X509Certificate2 Cert = ExtraeCertificado(NombreSujetoCer);

            if (Cert == null)
            {
                Console.WriteLine("No se ha recibido certificado.");
                Environment.Exit(1);
            }
            Console.WriteLine("Encontrado certificado: " + Cert.Subject);
        }

        static X509Certificate2 ExtraeCertificado(string Nombre)
        {
            // Fase 1. Abrir el almacén y extraer sus certificados
            X509Store Almacen = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);
            Console.WriteLine("Name: " + Almacen.Name);
            Console.WriteLine("Location: " + Almacen.Location);

            Almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection ColeCert = Almacen.Certificates;
            Almacen.Close();

            foreach(X509Certificate2 Cert in ColeCert)
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
    }
}
