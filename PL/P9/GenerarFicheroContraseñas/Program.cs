using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GenerarFicheroContraseñas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ListaU LU = new ListaU();
            LU.Lista.Add(new User("Antonio", "conA"));
            LU.Lista.Add(new User("Benito", "conB"));
            LU.Lista.Add(new User("Carlos", "conC"));
            LU.Lista.Add(new User("David", "conD"));
            LU.Lista.Add(new User("Eduardo", "conE"));
            
            LU.VerLista();
            LU.GuardaListaBin("zz_Usuarios.bin");
            LU.GuardaListaTxt("zz_Usuarios.txt");
            LU.GuardaListaXml("zz_Usuarios.xml");
        }
    }

    public class User
    {
        public char[] Nombre { get; set; }
        public byte[] Salt { get; set; }
        public byte[] ResuContra { get; set; }

        private const int LongitudNombre = 16;
        private const int LongitudSalt = 16;
        private const int LongitudResuContra = 32;

        public User(string NuevoNombre, string NuevaContra)
        {
            Nombre = new char[LongitudNombre];
            Salt = new byte[LongitudSalt];
            ResuContra = new byte[LongitudResuContra];

            // Guardar el nombre proporcionado en el array "Nombre"
            // Se asume que la longitud del nombre es menor o igual que la longitud del array
            // Para el relleno, se confía en que se haya inicializado el array con espacios en blanco
            for (int i = 0; i < NuevoNombre.Length; i++)
            {
                Nombre[i] = NuevoNombre[i];
            }

            new RNGCryptoServiceProvider().GetBytes(Salt); // Rellenar el array Salt con bytes aleatorios
            
            Stopwatch Crono = new Stopwatch();
            Crono.Start();
            ResuContra = new Rfc2898DeriveBytes(NuevaContra, Salt, 10000).GetBytes(LongitudResuContra);
            // byte[] SaltYContra = new byte[Salt.Length + NuevaContra.Length];
            // Salt.CopyTo(SaltYContra, 0);
            // Encoding.ASCII.GetBytes(NuevaContra).CopyTo(SaltYContra, Salt.Length);
            // ResuContra = new SHA256Managed().ComputeHash(SaltYContra);
            Crono.Stop();
            Console.WriteLine("Tiempo de ejecución: " + ((double)(1000000L * Crono.ElapsedTicks)) / Stopwatch.Frequency + " µs");
            // la constante por la que hay que multiplicar el número de iteraciones para obtener el tiempo de cálculo del resumen es 1/125
        }
    }

    public class ListaU
    {
        public List<User> Lista = new List<User>();

        // Método que muestra el nombre de todos los usuarios que contiene la Lista en la consola
        public void VerLista()
        {
            Console.WriteLine("Lista de usuarios: ");
            foreach (User u in Lista)
            {
                Console.WriteLine(u.Nombre);
            }
        }

        // Método que almacena los tres datos de cada usuario en el fichero indicado.
        // Este método debe guardar los datos en formato binario.
        // Se utiliza BinaryWriter con un Encoding para los caracteres de tipo Unicode
        public void GuardaListaBin(string NombreFich)
        {
            FileStream Fs = new FileStream(NombreFich, FileMode.Create);
            BinaryWriter Bw = new BinaryWriter(Fs, Encoding.Unicode);
            foreach (User u in Lista)
            {
                Bw.Write(u.Nombre);
                Bw.Write(u.Salt);
                Bw.Write(u.ResuContra);
            }
            Bw.Close();
            Fs.Close();
        }

        // Método que almacena los tres datos de cada usuario en el fichero indicado.
        // Este método debe guardar los datos en formato de texto.
        // Se utiliza StreamWriter con un Encoding para los caracteres de tipo ASCII.
        // Los datos que son intrínsecamente binarios como Salt o ResuContra hay que 
        // traducirlos a una representación textual. Informalmente, se puede almacenar
        // en formato de texto los dos dígitos hexadecimales que representan a cada byte.
        // Más formalmente, se debe codificar cada dato en formato Base64 usando el método
        // Convert.ToBase64String.
        public void GuardaListaTxt(string NombreFich)
        {
            FileStream Fs = new FileStream(NombreFich, FileMode.Create);
            StreamWriter Sw = new StreamWriter(Fs, Encoding.ASCII);
            foreach (User u in Lista)
            {
                Sw.Write(u.Nombre);
                Sw.Write(Convert.ToBase64String(u.Salt));
                Sw.WriteLine(Convert.ToBase64String(u.ResuContra));
            }
            Sw.Close();
            Fs.Close();
        }

        public void GuardaListaXml(string NombreFich)
        {
            FileStream Fs = new FileStream(NombreFich, FileMode.Create);
            XmlWriterSettings Xws = new XmlWriterSettings();
            Xws.Indent = true;
            Xws.Encoding = Encoding.UTF8;
            XmlWriter Xw = XmlWriter.Create(Fs, Xws);
            Xw.WriteStartDocument();
            Xw.WriteStartElement("Lista");
            foreach (User u in Lista)
            {
                Xw.WriteStartElement("Usuario");
                Console.WriteLine(new string(u.Nombre));
                Xw.WriteElementString("Nombre", new string(u.Nombre));
                Xw.WriteElementString("Salt", Convert.ToBase64String(u.Salt));
                Xw.WriteElementString("ResuContra", Convert.ToBase64String(u.ResuContra));
                Xw.WriteEndElement();
            }
            Xw.WriteEndElement();
            Xw.WriteEndDocument();
            Xw.Close();
            Fs.Close();
        }
    }
}
