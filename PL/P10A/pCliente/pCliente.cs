// pCliente - Cliente TCP Secuencial simple - Se ejecuta con pServidor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Espacios de nombres añadidos
using System.Net;
using System.Net.Sockets;

namespace pCliente
{
    class Program
    {
        static void Main(string[] args)
        {
            // Buferes de memoria para enviar peticiones y recibir respuestas
            int TamBuf = 10;
            Byte[] Peticion = new Byte[TamBuf];
            Byte[] Respuesta = new Byte[TamBuf];

            int BytesRecibidos, BytesEnviados;

            for (int Conex = 1; Conex <= 5; Conex++)
            {
                // Crear el socket de servicio
                Socket SocServicio = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Seleccionar la direccion del servidor remoto
                // Descomentar una de las siguienes opciones:
                IPAddress DirRemota = IPAddress.Loopback;
                //IPAddress DirRemota = IPAddress.Parse("156.35.163.196");

                // Seleccionar el puerto remoto
                int PuertoRemoto = 2459;

                // Crear el Extremo Remoto de una comunicación o conexión
                IPEndPoint EPRemoto= new IPEndPoint(DirRemota, PuertoRemoto);

                // Conectar el socket
                SocServicio.Connect(EPRemoto);

                // Información al usuario
                Console.WriteLine("Conexion {0} con {1}:{2}", Conex,
                    DirRemota.ToString(), PuertoRemoto);

                // Preparar la petición
                for (int i = 0; i < TamBuf; i++) Peticion[i] = 0;
                Peticion[0] = (Byte)(Conex % 256);

                // Enviar una petición (cadena) al servidor
                BytesEnviados = SocServicio.Send(Peticion);
                Console.Write("-Enviados {0} bytes:", BytesEnviados);
                for (int b = 0; b < BytesEnviados; b++) Console.Write(" {0:X2}", Peticion[b]);
                Console.WriteLine();

                // Recibir una respuesta (cadena) del servidor
                BytesRecibidos = SocServicio.Receive(Respuesta);
                Console.Write("Recibidos {0} bytes:", BytesRecibidos);
                for (int b = 0; b < BytesRecibidos; b++) Console.Write(" {0:X2}", Respuesta[b]);
                Console.WriteLine("\n");

                // Cerrar la conexión
                SocServicio.Close();
            } // for()
        } // Main()
    } // class Program
} // namespace SecuenClien
