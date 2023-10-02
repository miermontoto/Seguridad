using Apoyo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Práctica4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var a = new Ayuda();
            var rcsp = new RSACryptoServiceProvider();
            rcsp.KeySize = 1024;

            Console.WriteLine("Tamaño de clave: " + rcsp.KeySize);
            Console.WriteLine("Tamaños legales de la clave: " + a.ArrayToString(rcsp.LegalKeySizes));
            Console.WriteLine("Debe persistir en el proveedor?: " + rcsp.PersistKeyInCsp);

            // Primer método: exportar la clave a formato XML
            string xml = rcsp.ToXmlString(true);
            string str = rcsp.ToString();
            Console.WriteLine("Clave en formato XML: " + xml);
            Console.WriteLine("Clave en formato string: " + str);

            // Segundo método: exportar los parámetros del proveedor a una estructura RSAParameters
            RSAParameters rsaParams = rcsp.ExportParameters(true);

            Console.Write("Exponente privado: ");
            a.WriteHex(rsaParams.D, rsaParams.D.Length);
            Console.Write("Exponente público: ");
            a.WriteHex(rsaParams.Exponent, rsaParams.Exponent.Length);
            Console.Write("Módulo: ");
            a.WriteHex(rsaParams.Modulus, rsaParams.Modulus.Length);
            Console.Write("Primo P: ");
            a.WriteHex(rsaParams.P, rsaParams.P.Length);
            Console.Write("Primo Q: ");
            a.WriteHex(rsaParams.Q, rsaParams.Q.Length);

            // Guardar cada uno en un fichero "textual"
            a.GuardaTxt("d.txt", rsaParams.D);
            a.GuardaTxt("e.txt", rsaParams.Exponent);
            a.GuardaTxt("n.txt", rsaParams.Modulus);
            a.GuardaTxt("p.txt", rsaParams.P);
            a.GuardaTxt("q.txt", rsaParams.Q);

            // Tercer método: exportar las claves a un blob
            byte[] blob = rcsp.ExportCspBlob(true);
            a.GuardaBufer("zz_BlobRSA_Priva.bin", blob);

            byte[] blobPublic = rcsp.ExportCspBlob(false);
            a.GuardaBufer("zz_BlobRSA_Publi.bin", blobPublic);

            // Liberar recursos
            rcsp.Dispose();
            rcsp.Clear();

            // Crear un nuevo proveedor y cargar el blob
            var rcsp2 = new RSACryptoServiceProvider();
            byte[] blob2 = new byte[blob.Length];
            a.CargaBufer("zz_BlobRSA_Priva.bin", blob2);
            rcsp2.ImportCspBlob(blob);
            Console.WriteLine("Clave privada cargada desde el blob: " + rcsp2.ToXmlString(true));
        }
    }
}
