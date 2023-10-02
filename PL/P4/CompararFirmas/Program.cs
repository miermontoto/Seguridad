using Apoyo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FirmaDigital
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var a = new Ayuda();
            var Mensaje = new byte[64];
            for (int i = 0; i < Mensaje.Length; i++)
            {
                Mensaje[i] = (byte) i;
            }

            var rcsp = new RSACng(1024);
            string AlgResumen = "SHA384";
            HashAlgorithmName NomAlgRes = new HashAlgorithmName(AlgResumen);
            RSASignaturePadding RellenoFirma = RSASignaturePadding.Pss;

            // Usa la sobrecarga del método SignData que usa tres parámetros: el mensaje, el nombre del algoritmo de resumen y el relleno de la firma.
            byte[] Firma = rcsp.SignData(Mensaje, NomAlgRes, RellenoFirma);

            bool ValidezMensaje = rcsp.VerifyData(Mensaje, Firma, NomAlgRes, RellenoFirma);
            Console.WriteLine("Validez del mensaje: " + ValidezMensaje);

            rcsp.Dispose();
            rcsp.Clear();
        }
    }
}
