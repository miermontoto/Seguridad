using Apoyo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DescifrarRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var a = new Ayuda();
            var rcsp = new RSACryptoServiceProvider(1024);

            var blob = new byte[a.BytesFichero("zz_BlobRSA_Priva.bin")];
            a.CargaBufer("zz_BlobRSA_Priva.bin", blob);
            rcsp.ImportCspBlob(blob);

            byte[] bytesCifrados = new byte[a.BytesFichero("zz_TextoCifrado.bin")];
            a.CargaBufer("zz_TextoCifrado.bin", bytesCifrados);

            byte[] bytesDescifrados = rcsp.Decrypt(bytesCifrados, false);
            Console.Write("Texto descifrado: ");
            a.WriteHex(bytesDescifrados, bytesDescifrados.Length);

            rcsp.Dispose();
            rcsp.Clear();
        }
    }
}
