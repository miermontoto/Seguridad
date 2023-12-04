using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VerificacionUsuarioContraseña
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Introduce el nombre de usuario: ");
            string nombre = Console.ReadLine();
            Console.WriteLine("Introduce la contraseña: ");
            string contra = Console.ReadLine();
            UsuContra uc = new UsuContra();
            int resultado = uc.Verifica(nombre, contra);

            switch (resultado)
            {
                case 0:
                    Console.WriteLine("Usuario y contraseña correctos");
                    break;
                case 1:
                    Console.WriteLine("Usuario incorrecto");
                    break;
                case 2:
                    Console.WriteLine("Contraseña incorrecta");
                    break;
                default:
                    Console.WriteLine("Error desconocido");
                    break;
            }
        }
    }

    public class UsuContra
    {
        private const int MaxUsu = 5;
        private const int LongitudNombre = 16;
        private const int LongitudSalt = 16;
        private const int LongitudResuContra = 32;

        public char[] Nombre = new char[LongitudNombre];
        public char[] Salt = new char[LongitudSalt];
        public char[] ResuContra = new char[LongitudResuContra];

        // Será devuelto 0 si Nombre y ContraIn son válidos, 1 si NombreIn es desconocido , 2 si ContraIn es incorrecto
        public int Verifica(string NombreIn, string ContraIn)
        {
            // 1. Copiar el string NombreIn en un array de caracteres.
            char[] NombreInChar = new char[LongitudNombre];

            for (int i = 0; i < NombreIn.Length; i++)
            {
                NombreInChar[i] = NombreIn[i];
            }

            // 2. Buscar el NombreIn en el fichero de contraseñas, utilizando StreamReader.
            FileStream Fs = new FileStream("zz_Usuarios.txt", FileMode.Open, FileAccess.Read);
            StreamReader Sr = new StreamReader(Fs, Encoding.ASCII);

            bool Encontrado = false;
            int NumCarSalt = (int) (4 * Math.Ceiling(LongitudSalt / 3.0));
            int NumCarResuContra = (int) (4 * Math.Ceiling(LongitudResuContra / 3.0));
            char[] NombreFich = new char[LongitudNombre];
            char[] SaltFich = new char[NumCarSalt];
            char[] ResuFich = new char[NumCarResuContra];
            do
            {
                // Se lee un registro de un usuario (Nombre, Salt, ResuContra)
                Sr.Read(NombreFich, 0, LongitudNombre);

                Sr.Read(SaltFich, 0, NumCarSalt);
                Sr.Read(ResuFich, 0, NumCarResuContra);
                Sr.ReadLine();

                // Se compara el NombreIn con el NombreFich
                Encontrado = NombreFich.SequenceEqual(NombreInChar);
            }
            while (!Encontrado && !Sr.EndOfStream);
            Sr.Close();
            Fs.Close();

            if (!Encontrado) return 1;

            byte[] Salt = Convert.FromBase64CharArray(SaltFich, 0, SaltFich.Length);
            byte[] ResuContra = Convert.FromBase64CharArray(ResuFich, 0, ResuFich.Length);

            Rfc2898DeriveBytes DeriveBytes = new Rfc2898DeriveBytes(ContraIn, Salt, 10000);
            byte[] ResuContraIn = DeriveBytes.GetBytes(LongitudResuContra);

            if (ResuContraIn.SequenceEqual(ResuContra)) return 0;
            else return 2;
        }
    }
}
