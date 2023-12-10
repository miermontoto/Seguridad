// pUdpSer - Servidor UDP

// Generador de tráfico UDP intenso
// Usar con pUdpCli1 y pUdpCliN

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using System.Threading;

namespace Seguridad
{
    class pUdpSer
    {
        static void Main(string[] args)
        {
            // Creación del socket
            Socket Soc = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

            // Extremo local (servidor)
            IPEndPoint ExtLocalIP = new IPEndPoint(IPAddress. Any, 9050);
            EndPoint ExtLocal = (EndPoint)ExtLocalIP;

            // Enlace el socket al extremo local
            Soc.Bind(ExtLocal);

            // Extremo remoto (cliente)
            IPEndPoint ExtRemotoIP = new IPEndPoint(IPAddress.Any, 0);
            EndPoint ExtRemoto = (EndPoint)(ExtRemotoIP);

            // Variables para controlar los bytes recibidos y enviados
            int NumBytesRecibidos, NumBytesEnviados;

            // Buferes de memoria para enviar peticiones y recibir respuestas
            int TamBuf = 512;
            byte[] BufRec;

            int NumPet = 1;
            while (true)
            {
                // Recibir una petición de un cliente
                Console.WriteLine("Esperando peticion {0}", NumPet); // DEBUG
                BufRec = new byte[TamBuf];

                NumBytesRecibidos = Soc.ReceiveFrom(BufRec, ref ExtRemoto);

                Console.Write("El cliente {0} ha enviado: ", ExtRemoto);
                for (int b = 0; b < NumBytesRecibidos; b++) Console.Write(" {0:X2}", BufRec[b]);
                Console.WriteLine();

                Thread.Sleep(10);

                // Devolver el eco del mensaje recibido
                NumBytesEnviados = Soc.SendTo(BufRec, NumBytesRecibidos,
                    SocketFlags.None, ExtRemoto);

                NumPet++;
            } // while(true)

        } // Main()
    } // class
} // namespace