using System;
using Apoyo;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CifrarDescifrarRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var a = new Ayuda();
            var rcsp = new RSACryptoServiceProvider(1024);

            Console.WriteLine("Longitud de la llave: " + rcsp.KeySize);
            var blob = new byte[a.BytesFichero("zz_BlobRSA_Publi.bin")];
            a.CargaBufer("zz_BlobRSA_Publi.bin", blob);
            rcsp.ImportCspBlob(blob);

            // Crear un array de bytes en memoria para almacenar el texto plano a cifrar.
            byte[] textoPlano = new byte[64];
            // Rellenar de números consecutivos.
            for (int i = 0; i < textoPlano.Length; i++)
            {
                textoPlano[i] = (byte)(i + 1);
            }

            // Declarar un array de bytes en memoria para almacenar el texto cifrado.
            byte[] textoCifrado = new byte[rcsp.KeySize / 8];

            // Cifrar el texto plano y almacenarlo en el array de bytes cifrado.
            textoCifrado = rcsp.Encrypt(textoPlano, false);

            // Mostrar el texto cifrado por consola y guardarlo en un fichero.
            Console.WriteLine("Texto cifrado: " + Encoding.UTF8.GetString(textoCifrado));
            a.GuardaBufer("zz_TextoCifrado.bin", textoCifrado);

            rcsp.Dispose();
            rcsp.Clear();
        }
    }
}
