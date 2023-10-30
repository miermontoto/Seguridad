using Apoyo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: aes <bin> <output>");
                Environment.Exit(1);
            }

            string NombreFicheroCifrado = args[0];
            string NombreFicheroDescifrado = args[1];
            Console.WriteLine("Nombre del fichero cifrado: " + NombreFicheroCifrado);
            Console.WriteLine("Nombre del fichero descifrado: " + NombreFicheroDescifrado);
            Ayuda a = new Ayuda();

            // Crear un proveedor AES usando la clase AesManaged
            AesManaged Proveedor = new AesManaged();
            Proveedor.KeySize = 256;
            Proveedor.Mode = CipherMode.CBC;
            Proveedor.Padding = PaddingMode.ISO10126;

            byte[] Clave = new byte[32];
            byte[] VI = new byte[16];
            for (int i = 0; i < Clave.Length; i++) Clave[i] = (byte)(i % 256);
            for (int i = 0; i < VI.Length; i++) VI[i] = (byte)((i + 160) % 256);

            Proveedor.Key = Clave;
            Proveedor.IV = VI;

            // Streams de lectura
            FileStream FsCifrado = new FileStream(NombreFicheroCifrado, FileMode.Open);
            ICryptoTransform Descifrador = Proveedor.CreateDecryptor();
            CryptoStream CsCifrado = new CryptoStream(FsCifrado, Descifrador, CryptoStreamMode.Read);
            BinaryReader BrCifrado = new BinaryReader(CsCifrado, Encoding.UTF8);

            // Streams de escritura
            FileStream FsDescifrado = new FileStream(NombreFicheroDescifrado, FileMode.Create);
            BinaryWriter BrDescifrado = new BinaryWriter(FsDescifrado, Encoding.UTF8);

            Console.WriteLine("Proveedor y streams creados");
            long Longitud = a.BytesFichero(NombreFicheroCifrado);
            byte[] BinDescifrado = new byte[999999];
            Console.WriteLine("Longitud del archivo a descifrar: " + Longitud);

            // Descifrado
            int Read = -1;
            int Index = 0;
            while(Read != 0) // Se recupera de 10 en 10 hasta terminar de leer
            {
                byte[] Buffer = new byte[10];
                Read = BrCifrado.Read(Buffer, 0, 10);
                foreach(byte b in Buffer)
                {
                    BinDescifrado[Index] = b;
                    Index++;
                }
            }
            //BrCifrado.Read(BinDescifrado, 0, BinDescifrado.Length);
            Console.WriteLine("Descifrado completado");

            // Escritura
            BrDescifrado.Write(BinDescifrado, 0, BinDescifrado.Length);
            Console.WriteLine("Escritura del contenido descifrado completado");

            // Limpieza de streams y demás
            BrCifrado.Close();
            BrDescifrado.Close();
            Descifrador.Dispose();
            FsCifrado.Close();
            FsDescifrado.Close();
            Console.WriteLine("Limpieza completada");
        }
    }
}
