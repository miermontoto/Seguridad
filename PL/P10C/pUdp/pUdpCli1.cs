// pUdpCli1 - Programa UDP Cliente
// 1 -> Usa 1 solo socket para enviar N peticiones al servidor
// Funciona con pUdpSer
// Asigna la IP de pUdpSer y despues compila pUdpCli1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;


namespace Seguridad
{
    class pUdpCli1
    {
        static void Main(string[] args)
        {
            // Creación del socket
            Socket Soc = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

            // Extremo remoto (servidor destino)
            // Servidor al que hay que enviar el mensaje
            IPEndPoint ExtRemDestinoIP = new IPEndPoint(
                IPAddress.Parse("156.35.151.20"), 9050);
            EndPoint ExtRemDestino = (EndPoint)ExtRemDestinoIP;

            // Extremo remoto (servidor que responde)
            // Extremo vacio para que lo rellene el servidor
            IPEndPoint ExtRemRespondeIP = new IPEndPoint(
                IPAddress.Any, 0);
            EndPoint ExtRemResponde = (EndPoint)ExtRemRespondeIP;

            // Extremo local
            IPEndPoint ExtLocIP = new IPEndPoint(
                IPAddress.Any, 0);
            EndPoint ExtLoc = (EndPoint)ExtLocIP;

            // Variables para controlar los bytes enviados y recibidos
            int NumBytesEnviados = -1, NumBytesRecibidos = -1;

            // Buferes de memoria para enviar peticiones y recibir respuestas
            int TamBuf = 10;
            Byte[] Peticion = new Byte[TamBuf];
            Byte[] Respuesta = new Byte[TamBuf];

            for (int Com = 1; Com <= 100; Com++)
            {
                // Preparar la petición
                for (int i = 0; i < TamBuf; i++) Peticion[i] = 0;
                Peticion[0] = (Byte)(Com % 256);

                // Enviar una petición (cadena) al servidor
                NumBytesEnviados = Soc.SendTo(Peticion, ExtRemDestino);

                Console.Write("-Enviados {0} bytes:", NumBytesEnviados);
                for (int b = 0; b < NumBytesEnviados; b++) Console.Write(" {0:X2}", Peticion[b]);
                Console.WriteLine();

                // Recibir una respuesta (cadena) del servidor
                NumBytesRecibidos = Soc.ReceiveFrom(Respuesta, ref ExtRemResponde);

                Console.Write("Recibidos {0} bytes:", NumBytesRecibidos);
                for (int b = 0; b < NumBytesRecibidos; b++) Console.Write(" {0:X2}", Respuesta[b]);
                Console.WriteLine("\n");
            }
            Console.WriteLine("Parando el cliente ...");
            Soc.Shutdown(SocketShutdown.Both);
            Soc.Close();
        } // Main()
    } // class
} // namespace