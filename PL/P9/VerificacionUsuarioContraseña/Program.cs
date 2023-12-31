﻿using System;
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

            switch(resultado)
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

            // 2. Buscar el NombreIn en el fichero de contraseñas, utilizando BinaryReader.
            FileStream Fs = new FileStream("zz_Usuarios.bin", FileMode.Open, FileAccess.Read);
            BinaryReader Br = new BinaryReader(Fs, Encoding.Unicode);

            bool Encontrado = false;
            char[] NombreFich;
            byte[] SaltFich;
            byte[] ResuFich;
            do
            {
                // Se lee un registro de un usuario (Nombre, Salt, ResuContra)
                NombreFich = Br.ReadChars(LongitudNombre);
                SaltFich = Br.ReadBytes(LongitudSalt);
                ResuFich = Br.ReadBytes(LongitudResuContra);

                // Se compara el NombreIn con el NombreFich
                Encontrado = NombreFich.SequenceEqual(NombreInChar);
            }
            while(!Encontrado && Br.BaseStream.Position < Br.BaseStream.Length);
            Br.Close();
            Fs.Close();

            if (!Encontrado) return 1;

            Rfc2898DeriveBytes DeriveBytes = new Rfc2898DeriveBytes(ContraIn, SaltFich, 10000);
            byte[] ResuContraIn = DeriveBytes.GetBytes(LongitudResuContra);

            if (ResuContraIn.SequenceEqual(ResuFich)) return 0;
            else return 2;
        }
    }
}
