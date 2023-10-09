using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AñadirEliminarCertificados
{
    internal class Program
    {
        static void Main(string[] args)
        {
            X509Store Almacen = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            Almacen.Open(OpenFlags.ReadWrite);

            var Cert1 = new X509Certificate2("zpACas.cer");
            var Cert2 = new X509Certificate2("zpSERas.cer");
            var Cert3 = new X509Certificate2("zpUSUas.cer");

            Almacen.Add(Cert1);
            X509Certificate2Collection ColeCert = new X509Certificate2Collection();
            ColeCert.Add(Cert2);
            ColeCert.Add(Cert3);

            Almacen.AddRange(ColeCert);

            VerCerts(Almacen);
            Almacen.Remove(Cert1);
            VerCerts(Almacen);
            Almacen.RemoveRange(ColeCert);
            VerCerts(Almacen);
            Almacen.Close();
        }

        static void VerCerts(X509Store AL)
        {
            // Mostrar nombre del almacén
            Console.WriteLine("Almacen: {0}", AL.Name);

            foreach (var cert in AL.Certificates)
            {
                Console.WriteLine("Certificado: {0}", cert.Subject);
            }
            
            if (AL.Certificates.Count == 0)
            {
                Console.WriteLine("No hay certificados en el almacén");
            }   
        }
    }
}
