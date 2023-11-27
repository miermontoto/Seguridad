using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LecturaEventos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<EventRecord> Records = LeeFicheroEventos("Fichero.evtx");
            Console.WriteLine("Eventos en el fichero: " + Records.Count);
            List<EventRecord> RecordsFiltrado = FiltroIDs(Records, new long[1] { 4950 });
            Console.WriteLine("Eventos filtrados: " +  RecordsFiltrado.Count);
            //VerPropEven(RecordsFiltrado);
            VerPerFwDesactivado(Records, "Público");
            VerPerFwDesactivado(Records, "Privado");
            VerPerFwDesactivado(Records, "Dominio");
        }

        static List<EventRecord> LeeFicheroEventos(string NombreFichero)
        {
            EventLogReader LectorEventos = new EventLogReader(NombreFichero, PathType.FilePath);
            List<EventRecord> ListaEventos = new List<EventRecord>();
            EventRecord e;
            while ((e = LectorEventos.ReadEvent()) != null)
            {
                ListaEventos.Add(e);
            }

            return ListaEventos;
        }

        static List<EventRecord> FiltroIDs(List<EventRecord> ListaEnt, long[] ListaID)
        {
            List<EventRecord> ListaSal = new List<EventRecord>();
            foreach (var Event in ListaEnt)
            {
                foreach (var ID in ListaID)
                {
                    if (ID == Event.Id)
                    {
                        ListaSal.Add(Event);
                        break;
                    }
                }
            }
            return ListaSal;
        }

        static void VerPropEven(List<EventRecord> ListaEnt)
        {
            foreach (EventRecord record in ListaEnt)
            {
                Console.WriteLine(record.FormatDescription());
            }
        }

        static void VerPerFwDesactivado(List<EventRecord> LE, string PerfilFW)
        {
            PeriodoDesactivacion PerDes = null;
            List<PeriodoDesactivacion> LisPerDes = new List<PeriodoDesactivacion>();
            foreach (EventRecord E in LE)
            {
                if (E.Id != 4950) continue;

                string Perfil = (string) E.Properties[0].Value;
                string TipoConfig = (string) E.Properties[1].Value;
                string ValorConfig = (string) E.Properties[2].Value;
                if (Perfil != PerfilFW) continue;
                if (TipoConfig != "Habilitar Firewall de Windows Defender") continue;

                if (PerDes != null && ValorConfig == "Sí")
                {
                    PerDes.Tfin = E.TimeCreated.GetValueOrDefault();
                    PerDes.Duracion = PerDes.Tfin - PerDes.Tini;
                    LisPerDes.Add(PerDes);
                    PerDes = null;
                }
                else if (PerDes == null && ValorConfig == "No")
                {
                    PerDes = new PeriodoDesactivacion();
                    PerDes.Tini = E.TimeCreated.GetValueOrDefault();
                } else
                {
                    Console.WriteLine("Error inesperado");
                    Environment.Exit(1);
                }
            }

            foreach (PeriodoDesactivacion Periodo in LisPerDes)
            {
                Console.WriteLine($"Período de desactivación desde {Periodo.Tini} hasta {Periodo.Tfin} y duración {Periodo.Duracion}");
            }
        }
    }

    internal class PeriodoDesactivacion
    {
        public DateTime Tini = new DateTime();
        public DateTime Tfin = new DateTime();
        public TimeSpan Duracion = new TimeSpan();
    }
}
