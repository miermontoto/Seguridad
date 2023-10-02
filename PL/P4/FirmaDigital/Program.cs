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

            // Fase 1: cálculo del hash
            var scsp = new SHA256CryptoServiceProvider();
            Console.WriteLine("Tamaño del hash: " + scsp.HashSize);

            byte[] Resumen = scsp.ComputeHash(Mensaje);
            Console.Write("Resumen: ");
            a.WriteHex(Resumen, Resumen.Length);
            scsp.Dispose();
            scsp.Clear();

            // Fase 2: proveedor RSA para cifrar el valor del hash
            var rcsp = new RSACryptoServiceProvider(1024);
            Console.WriteLine("Tamaño de la llave: " + rcsp.KeySize);
            byte[] blob = new byte[a.BytesFichero("zz_BlobRSA_Priva.bin")];
            a.CargaBufer("zz_BlobRSA_Priva.bin", blob);
            rcsp.ImportCspBlob(blob);

            // Fase 3: generar la firma
            byte[] Firma = rcsp.SignHash(Resumen, CryptoConfig.MapNameToOID("SHA256"));
            Console.Write("Firma: ");
            a.WriteHex(Firma, Firma.Length);
            byte[] Firma2 = rcsp.SignHash(Resumen, CryptoConfig.MapNameToOID("SHA256"));
            Console.Write("Firma2: ");
            a.WriteHex(Firma2, Firma2.Length);

            // Comprobar que la firma se puede obtener en un paso con SignData
            byte[] Firma3 = rcsp.SignData(Mensaje, CryptoConfig.MapNameToOID("SHA256"));
            Console.Write("Firma3: ");
            a.WriteHex(Firma3, Firma3.Length);

            // Fase 4. Volcar en ficheros
            a.GuardaBufer("zz_Firma.bin", Firma);
            a.GuardaBufer("zz_Mensaje.bin", Mensaje);

            // Fase 5. Verificar la firma

            // 1. Tres instrucciones que modifiquen un byte de los arrays
            // de Mensaje, Resumen y Firma.
            //Mensaje[2] = 0;
            Resumen[3] = 0;
            //Firma[4] = 0;

            // 2. Verificar la validez de la firma contra el resumen mediante un bool
            // y asignándole el resultado de la verificación.
            bool ValidezResumen = rcsp.VerifyHash(Resumen, CryptoConfig.MapNameToOID("SHA256"), Firma);
            Console.WriteLine("Validez del resumen: " + ValidezResumen);

            // 3. Verificar la validez de la firma contra el mensaje
            bool ValidezMensaje = rcsp.VerifyData(Mensaje, CryptoConfig.MapNameToOID("SHA256"), Firma);
            Console.WriteLine("Validez del mensaje: " + ValidezMensaje);

            rcsp.Dispose();
            rcsp.Clear();
        }
    }
}
