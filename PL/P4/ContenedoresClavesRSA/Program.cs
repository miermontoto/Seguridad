using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ContenedoresClavesRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int KeySize = 1024;

            // Fase 1. Crear un contenedor de una clave RSA y ver sus propiedades
            var ParamCSP = new CspParameters();
            ParamCSP.ProviderType = 1; // PROV_RSA_FULL
            ParamCSP.ProviderName = "Microsoft Strong Cryptographic Provider";
            ParamCSP.KeyContainerName = "ALEXZELOCO";
            ParamCSP.KeyNumber = (int) KeyNumber.Exchange;
            ParamCSP.Flags = CspProviderFlags.UseMachineKeyStore;

            var ProvRSA = new RSACryptoServiceProvider(KeySize, ParamCSP);
            ProvRSA.PersistKeyInCsp = true;
            Console.WriteLine("Key size: " + ProvRSA.KeySize);
            Console.WriteLine("Persisted Key: " + ProvRSA.PersistKeyInCsp);
            Console.WriteLine("Public only: " + ProvRSA.PublicOnly);
            Console.WriteLine("Key exchange algorithm: " + ProvRSA.KeyExchangeAlgorithm);
            Console.WriteLine("Signature algorithm: " + ProvRSA.SignatureAlgorithm);
            Console.WriteLine("Use machine key store: " + ProvRSA.CspKeyContainerInfo.MachineKeyStore);
            Console.WriteLine();

            // Muestra en consola los elementos de la clave en formato XML
            Console.WriteLine("Clave pública: " + ProvRSA.ToXmlString(false));
            Console.WriteLine();

            // Muestra información sobre el contendor de la clave.
            var InfoConten = ProvRSA.CspKeyContainerInfo;
            VerContenedor(InfoConten);
            ProvRSA.Dispose();
            ProvRSA.Clear();

            // Fase 2. Cargar la clave RSA de un contenedor en un nuevo proveedor RSA
            var ProvRSA2 = new RSACryptoServiceProvider(KeySize, ParamCSP);
            Console.WriteLine("Clave pública: " + ProvRSA2.ToXmlString(false));
            Console.WriteLine();
            ProvRSA2.Dispose();
            ProvRSA2.Clear();

            // Fase 3. Observa el almacén en el que se creó el contenedor
            Console.WriteLine("Pulsa una tecla para continuar...");
            Console.ReadKey();

            // Fase 4. Eliminación de un contenedor
            var ProvRSA3 = new RSACryptoServiceProvider(KeySize, ParamCSP);
            ProvRSA3.PersistKeyInCsp = false;
            Console.WriteLine("Clave pública: " + ProvRSA3.ToXmlString(false));
            Console.WriteLine();
            ProvRSA3.Dispose();
            ProvRSA3.Clear();
        }

        static void VerContenedor(CspKeyContainerInfo cspKeyContainerInfo)
        {
            Console.WriteLine("Nombre del contenedor: " + cspKeyContainerInfo.KeyContainerName);
            Console.WriteLine("Proveedor: " + cspKeyContainerInfo.ProviderName);
            Console.WriteLine("Tipo de proveedor: " + cspKeyContainerInfo.ProviderType);
            Console.WriteLine("Almacén de claves: " + cspKeyContainerInfo.MachineKeyStore);
            Console.WriteLine("Clave persistente: " + cspKeyContainerInfo.Removable);
            Console.WriteLine("Clave protegida: " + cspKeyContainerInfo.Protected);
            Console.WriteLine("Clave de exportación: " + cspKeyContainerInfo.Exportable);
            Console.WriteLine("Clave de hardware: " + cspKeyContainerInfo.HardwareDevice);
            Console.WriteLine("Clave de acceso de usuario: " + cspKeyContainerInfo.UniqueKeyContainerName);
        }   
    }
}
