using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Credenciales;

namespace SerializacionXML
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Almacen Alma = new Almacen();
            Alma.usuarios.Add(new Usuario("Antonio", "conA"));
            Alma.usuarios.Add(new Usuario("Benito", "conB"));
            Alma.usuarios.Add(new Usuario("Carlos", "conC"));
            Alma.usuarios.Add(new Usuario("David", "conD"));
            Alma.usuarios.Add(new Usuario("Eduardo", "conE"));
            Alma.VerLista();

            string Fichero = "AlmacenUsuarios.xml";
            Alma.Serializar(Fichero);
            Console.WriteLine("Almacen serializado en {0}", Fichero);
            Almacen Alma2 = Almacen.Deserializar(Fichero);
            Console.WriteLine("Almacen deserializado de {0}", Fichero);
            Alma2.VerLista();
        }
    }
}
