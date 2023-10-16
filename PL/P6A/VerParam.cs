        internal static void VerParam(RSACryptoServiceProvider ProRSA, bool ExportaPriv)
        {
            RSAParameters ParamRSA = new RSAParameters();
            try
            {
                ParamRSA = ProRSA.ExportParameters(ExportaPriv);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPCION: " + e);
                Environment.Exit(1);
            }

            Console.WriteLine("\n--INICIO CLAVE RSA---------------------------");

            Console.WriteLine("Modulo (n) {0} bytes", ParamRSA.Modulus.Length);
            for (int i = 0; i < ParamRSA.Modulus.Length; i++)
                Console.Write("{0,2:X} ", ParamRSA.Modulus[i]);

            Console.WriteLine("\nExponente Publico (e) {0} bytes", ParamRSA.Exponent.Length);
            for (int i = 0; i < ParamRSA.Exponent.Length; i++)
                Console.Write("{0,2:X} ", ParamRSA.Exponent[i]);

            if (ExportaPriv)
            {
                Console.WriteLine("\nExponente Privado (d) {0} bytes", ParamRSA.D.Length);
                for (int i = 0; i < ParamRSA.D.Length; i++)
                    Console.Write("{0,2:X} ", ParamRSA.D[i]);
            }

            Console.WriteLine("\n--FIN CLAVE RSA------------------------------");
        }
