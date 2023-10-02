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
    internal class CifradorBytes
    {
        static void test(string[] args)
        {
            var a = new Ayuda();
            var TamClave = 32;
            var Clave = new byte[TamClave];
            for (int i = 0; i < Clave.Length; i++) Clave[i] = (byte)(i % 256);

            var VI = new byte[16];
            for (int i = 0; i < VI.Length; i++) VI[i] = (byte)((i + 160) % 256);

            byte[] TextoPlano = 
            { 
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
                0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
                0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
                0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
                0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F
            };

            var NombreFichero = "zz_TextoCifrado.bin";
            var acsp = new AesManaged();

            Console.WriteLine("KeySize: " + acsp.KeySize);
            Console.WriteLine("BlockSize: " + acsp.BlockSize);
            Console.WriteLine("Padding: " + acsp.Padding);
            Console.WriteLine("Mode: " + acsp.Mode);

            // Asignar a las propiedades Keysize, Padding y Mode nuevos valores. Comprobar que la asignación
            // se ha realizado, recuperando las propiedades y mostrándolas en la consola. Incluir en el programa,
            // mediante comentarios, los valores que es posible asignar a estas propiedades
            // (KeySize: 128, 192, 256; Padding: None, PKCS7, Zeros, ANSIX923, ISO10126; Mode: CBC, CFB, CTS, ECB, OFB)

            acsp.KeySize = 192;
            acsp.Padding = PaddingMode.PKCS7;
            acsp.Mode = CipherMode.ECB;

            Console.WriteLine();
            Console.WriteLine("KeySize: " + acsp.KeySize);
            Console.WriteLine("BlockSize: " + acsp.BlockSize);
            Console.WriteLine("Padding: " + acsp.Padding);
            Console.WriteLine("Mode: " + acsp.Mode);

            acsp.GenerateKey();
            Console.WriteLine();
            Console.WriteLine("Clave: ");
            a.WriteHex(acsp.Key, acsp.Key.Length);

            acsp.Key = Clave;
            Console.WriteLine();
            Console.WriteLine("Clave manual: ");
            a.WriteHex(acsp.Key, acsp.Key.Length);

            acsp.GenerateIV();
            Console.WriteLine();
            Console.Write("VI: ");
            a.WriteHex(acsp.IV, acsp.IV.Length);

            acsp.IV = VI;
            Console.WriteLine();
            Console.Write("VI manual: ");
            a.WriteHex(acsp.IV, acsp.IV.Length);

            // Proceso de cifrado de un array de bytes
            // 1) Definir un flujo de datos (FileStream) para almacenar el texto cifrado.
            // 2) Crear un objeto cifrador usando el método CreateEncryptor() del objeto AesCryptoServiceProvider.
            // 3) Crear un objeto de la clase CryptoStream, pasándole como parámetros el flujo de datos y el objeto cifrador.
            // 4) Cifrar la información mediante el método Write() del objeto CryptoStream.
            // 5) Flush y cerrar el flujo de datos.
            // 6) Liberar los recursos del objeto cifrador.
            // 7) Cerrar el FileStream.
            FileStream fsCifrado = new FileStream(NombreFichero, FileMode.Create);
            ICryptoTransform cifrador = acsp.CreateEncryptor();
            CryptoStream cs = new CryptoStream(fsCifrado, cifrador, CryptoStreamMode.Write);
            cs.Write(TextoPlano, 0, TextoPlano.Length);
            cs.Flush();
            cs.Close();
            cifrador.Dispose();
            fsCifrado.Close();

            Console.WriteLine();
            Console.WriteLine("Texto cifrado: ");
            var TextoCifrado = new byte[a.BytesFichero(NombreFichero)];
            a.CargaBufer(NombreFichero, TextoCifrado);
            a.WriteHex(TextoCifrado, TextoCifrado.Length);

            // Proceso de descifrado de un array de bytes
            // 1) Declarar un array de bytes (TextoDescifrado) con la longitud del texto cifrado.
            // 2) Definir un flujo de datos (FileStream) para leer el texto cifrado.
            // 3) Crear un objeto descifrador usando el método CreateDecryptor() del objeto AesCryptoServiceProvider.
            // 4) Crear un objeto de la clase CryptoStream, pasándole como parámetros el flujo de datos y el objeto descifrador.
            // 5) Descifrar la información mediante el método Read() del objeto CryptoStream.
            // 6) Cerrar el flujo de datos.
            // 7) Liberar los recursos del objeto descifrador.
            // 8) Cerrar el FileStream.
            // 9) Mostrar el texto descifrado en la consola.
            var TextoDescifrado = new byte[TextoPlano.Length];
            FileStream fsDescifrado = new FileStream("zz_TextoCifrado.bin", FileMode.Open);
            ICryptoTransform descifrador = acsp.CreateDecryptor();
            CryptoStream cs2 = new CryptoStream(fsDescifrado, descifrador, CryptoStreamMode.Read);
            cs2.Read(TextoDescifrado, 0, TextoDescifrado.Length);
            cs2.Flush();
            cs2.Close();
            descifrador.Dispose();
            fsDescifrado.Close();

            Console.WriteLine();
            Console.WriteLine("Texto descifrado: ");
            a.WriteHex(TextoDescifrado, TextoDescifrado.Length);
        }
    }
}
