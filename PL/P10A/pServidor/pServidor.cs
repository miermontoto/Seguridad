// pServidor - Servidor TCP Secuencial simple - Se ejecuta con pCliente

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Espacios de nombres añadidos
using System.Net;
using System.Net.Sockets;
using System.Net.Configuration; // Para ver el parámetro MaxConnection
using System.Threading; // Para usar Sleep()

namespace pServidor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Buferes de memoria para recibir peticiones y enviar respuestas
            int TamBuf = 10;
            Byte[] Peticion = new Byte[TamBuf];
            Byte[] Respuesta = new Byte[TamBuf];

            int BytesRecibidos, BytesEnviados, NumConex;
            
            // Seleccionar la direccion local para escuchar
            // Descomentar una de las siguienes opciones:
            // IPAddress DirLocal = IPAddress.Parse("156.35.163.196");
            IPAddress DirLocal = IPAddress.Any;

            // Seleccionar el puerto local para escuchar
            // Usar 0 para que el proveedor asigne uno entre 1024 y 5000
            int PuertoLocal = 2459;

            // Crear el Extremo Local de una comunicación o conexión
            IPEndPoint EPLocal = new IPEndPoint(DirLocal, PuertoLocal);

            // Crear el socket para escuchar peticiones
            Socket SocEscucha = new Socket
                (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Enlazar el socket al extremo local
            SocEscucha.Bind(EPLocal);
/*
            // Representa el número máximo de conexiones a un equipo remoto
            // Hay que agregar el ensamblado System.Configuration
            ConnectionManagementElement GestorConexion = new ConnectionManagementElement();
            Console.WriteLine("Numero maximo de conexiones: " + GestorConexion.MaxConnection);
*/
            // Poner al socket a escuchar peticiones de conexión
            SocEscucha.Listen(20);

            NumConex = 0;
            while(true) // Aceptar conexiones y procesar sus peticiones
            {
                // Console.WriteLine("Bloqueado esperando una conexion ..."); // Para depuración
                NumConex++;
                Socket SocServicio;
                try
                {
                    SocServicio = SocEscucha.Accept();
                }
                catch(Exception E)
                {
                    Console.WriteLine("EXCEPCION en Accept(): " + E.Message);
                    continue;
                }
                
                // Mostrar la dirección y el puerto del extremo remoto de la conexión
                IPEndPoint EPRemoto = (IPEndPoint) SocServicio.RemoteEndPoint;
                IPAddress DirRemota = EPRemoto.Address;
                int PuertoRemoto = EPRemoto.Port;
                Console.WriteLine("Conexion {0} de {1}:{2}", NumConex,
                    DirRemota.ToString(), PuertoRemoto);

                // Recibir una petición (cadena) del cliente
                BytesRecibidos = SocServicio.Receive(Peticion);
                Console.Write("Recibidos {0} bytes:", BytesRecibidos);
                for (int b = 0; b < BytesRecibidos; b++) Console.Write(" {0:X2}", Peticion[b]);
                Console.WriteLine();

                // Emular el procesamiento = Preparar la respuesta
                Thread.Sleep(1000); // Tiempo de auto-bloqueo expresado en milisegundos
                for (int i = 0; i < TamBuf; i++) Respuesta[i] = 0;
                Respuesta[TamBuf - 1] = (Byte)(NumConex % 256);

                // Enviar una respuesta (cadena) al cliente
                BytesEnviados = SocServicio.Send(Respuesta);
                Console.Write("-Enviados {0} bytes:", BytesEnviados);
                for (int b = 0; b < BytesEnviados; b++) Console.Write(" {0:X2}", Respuesta[b]);
                Console.WriteLine("\n");

                // Cerrar el socket que provee el servicio
                SocServicio.Close();
            } // while(true)

            // Cerrar el socket de escucha
            // SocEscucha.Close(); // instrucción inalcanzable

        } // Main()
    } // class Program
} // namespace SecuenServi
