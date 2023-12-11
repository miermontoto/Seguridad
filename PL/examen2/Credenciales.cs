// Credenciales.cs 

using System;
using System.Collections.Generic;

namespace Credenciales
{
    [Serializable]
    public class Almacen
    {
        public List<Usuario> Lista = new List<Usuario>();
    }

    [Serializable]
    public class Usuario
    {
        private static readonly int LongiSalt = 16; // Longitud del salt en bytes
        private static readonly int LongiResuContra = 32; // Longitud del resumen en bytes

        public string Nombre;
        public byte[] Salt = new byte[LongiSalt];
        public byte[] ResuContra = new byte[LongiResuContra];
    }
}