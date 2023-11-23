using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GenerarFicheroContraseñas
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ListaU LU = new ListaU();
            LU.IniUsu(0, new User("Antonio", "conA"));
            LU.IniUsu(1, new User("Benito", "conB"));
            LU.IniUsu(2, new User("Carlos", "conC"));
            LU.IniUsu(3, new User("David", "conD"));
            LU.IniUsu(4, new User("Eduardo", "conE"));
            
            LU.VerLista();
            LU.GuardaListaBin("zz_Usuarios.bin");
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
            ResuContra = new Rfc2898DeriveBytes(NuevaContra, Salt, 10000).GetBytes(LongitudResuContra); // Generar el resumen de la contraseña
        }
    }

    public class ListaU
    {
        private const int MaxUsu = 5;
        private User[] Lista = new User[MaxUsu];

        // Método que permite asociar un objeto Usuario a un índice de la lista
        public void IniUsu(int Indice, User Usuario)
        {
            Lista[Indice] = Usuario;
        }

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
        // Más formalmente, se debe codificar cada dato enformato Base64 usando el método
        // Convert.ToBase64String.
        public void GuardaListaTxt(string NombreFich)
        {
            
        }
    }
}
