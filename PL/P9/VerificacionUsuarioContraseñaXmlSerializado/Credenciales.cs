using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Credenciales
{
    [Serializable]
    public class Almacen
    {
        public List<Usuario> usuarios { get; set; } = new List<Usuario>();

        public void VerLista()
        {
            Console.WriteLine("Lista de usuarios: ");
            foreach (Usuario u in usuarios)
            {
                Console.WriteLine(u.Nombre);
            }
        }

        public void Serializar(string Fichero)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Almacen));
            using (FileStream fs = new FileStream(Fichero, FileMode.Create))
            {
                ser.Serialize(fs, this);
            }
        }

        public static Almacen Deserializar(string Fichero)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Almacen));
            using (FileStream fs = new FileStream(Fichero, FileMode.Open))
            {
                return (Almacen)ser.Deserialize(fs);
            }
        }
    }

    [Serializable]
    public class Usuario
    {
        public static int LongiSalt = 16;
        public static int LongiResuContra = 32;

        public string Nombre { get; set; }
        public byte[] Salt { get; set; } = new byte[LongiSalt];
        public byte[] ResuContra { get; set; } = new byte[LongiResuContra];

        public Usuario(string NuevoNombre, string NuevaContra)
        {
            Nombre = NuevoNombre;
            Salt = new byte[LongiSalt];
            ResuContra = new byte[LongiResuContra];

            new RNGCryptoServiceProvider().GetBytes(Salt);
            ResuContra = new Rfc2898DeriveBytes(NuevaContra, Salt, 10000).GetBytes(LongiResuContra);
        }

        public Usuario() { } // constructor vacío necesario para serialización
    }
}
