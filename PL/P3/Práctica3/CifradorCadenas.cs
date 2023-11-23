using Apoyo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    internal class CifradorCadenas
    {
        static void test(string[] args)
        {
            var a = new Ayuda();

            var Cadena = "Cadena a encriptar";
            var NombreFichero = "zz_CadenaCifrada.bin";
            var acsp = new AesManaged();
            var fsCifrado = new FileStream(NombreFichero, FileMode.Create);
            var cifrador = acsp.CreateEncryptor();
            var cs = new CryptoStream(fsCifrado, cifrador, CryptoStreamMode.Write);
            var stWriter = new StreamWriter(cs, Encoding.UTF8);
            stWriter.WriteLine(Cadena);
            stWriter.Close();
            stWriter.Dispose();
            cs.Dispose();
            cs.Close();
            cifrador.Dispose();
            fsCifrado.Dispose();
            fsCifrado.Close();
        }
    }
}
