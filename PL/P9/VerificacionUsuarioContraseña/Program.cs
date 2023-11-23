using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificacionUsuarioContraseña
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

            // 2. Buscar el NombreIn en el fichero de contraseñas, utilizando BinaryReader.
            FileStream Fs = new FileStream("zz_Usuarios.bin", FileMode.Open, FileAccess.Read);
            BinaryReader Br = new BinaryReader(Fs, Encoding.Unicode);

            bool Encontrado = false;
            char[] NombreFich, SaltFich, ResuFich;
            do
            {
                // Se lee un registro de un usuario (Nombre, Salt, ResuContra)
                NombreFich = Br.ReadChars(LongitudNombre);
                SaltFich = Br.ReadChars(LongitudSalt);
                ResuFich = Br.ReadChars(LongitudResuContra);

                // Se compara el NombreIn con el NombreFich
                Encontrado = NombreFich.SequenceEqual(NombreInChar);
            }
            while (!Encontrado);
            Br.Close();
            Fs.Close();

            if (!Encontrado) return 1;

            
        }
    }
}
