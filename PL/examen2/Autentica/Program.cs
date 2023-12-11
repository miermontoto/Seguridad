using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Credenciales;

namespace Autentica
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Solicitar el nombre y la contraseña
            Console.WriteLine("Introduce el nombre de usuario: ");
            string Nombre = Console.ReadLine();
            Console.WriteLine("Introduce la contraseña: ");
            string Contra = Console.ReadLine();

            // Declarar el objeto A de la clase Almacen y inicializarlo con un método que lo deserialice.
            Almacen A = Almacen.Deserializar("AlmacenUsuarios.xml");
            // Mostrar los nombres de los usuairos del Almacen por consola
            A.VerLista();

            // Escribir el resultado de la verificación
            switch (Verifica(Nombre, Contra, A))
            {
                case 0:
                    Console.WriteLine("Usuario y contraseña correctos.");
                    break;
                case 1:
                    Console.WriteLine("El usuario no existe.");
                    break;
                case 2:
                    Console.WriteLine("Contraseña incorrecta.");
                    break;
            }
        }

        // Método que recibe un nombre, una contraseña y un objeto de la clase Almacen
        static int Verifica(string NombreIn, string ContraIn, Almacen Alma)
        {
            // Declarar tres variables locales para referenciar los datos del usuario a comprobar
            string Nombre = "";
            byte[] Salt = new byte[Usuario.LongiSalt];
            byte[] ResuContra = new byte[Usuario.LongiResuContra];

            foreach (Usuario u in Alma.Lista) // para cada usuario en el almacén:
            {
                if (u.Nombre == NombreIn) // si el nombre coincide con el usuario que se busca, guardar datos y salir del bucle
                {
                    Nombre = u.Nombre;
                    Salt = u.Salt;
                    ResuContra = u.ResuContra;
                    break;
                }
            }

            if (Nombre == "") return 1; // si no se ha encontrado el nombre dentro de la lista, el usuario es desconocido -> 1

            byte[] Contra = Encoding.UTF8.GetBytes(ContraIn); // convertir contraseña a array de bytes
            byte[] ResuContraIntroducida = new byte[Usuario.LongiResuContra]; // variable local para referenciar el resumen de la contraseña

            using (Rfc2898DeriveBytes Derivador = new Rfc2898DeriveBytes(Contra, Salt, 1000)) // se emplean 1000 iteraciones como dice el enunciado del examen
            {
                ResuContraIntroducida = Derivador.GetBytes(Usuario.LongiResuContra); // calcular resumen original de la contraseña
            }

            if (!ResuContraIntroducida.SequenceEqual(ResuContra)) return 2; // si los resúmenes no coinciden, contraseña inválida -> 2
            return 0; // si se llega hasta aquí, el usuario y la contraseña son correctos -> 0
        }
    }
}
