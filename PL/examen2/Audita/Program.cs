using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audita
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<EventRecord> Records = LeeFicheroEventos("AV.evtx");
            Console.WriteLine("Eventos en el fichero: " + Records.Count);
            List<EventRecord> RecordsFiltrado = FiltroIDs(Records, new long[2] { 5000, 5001 });
            Console.WriteLine("Eventos filtrados: " + RecordsFiltrado.Count);
            //VerPropEven(RecordsFiltrado);
            VerPerAvDesactivado(Records, 5000, 5001);
        }

        // método que lee una lista de eventos a partir de un fichero de texto
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

        // método que filtra los eventos con los IDs introducidos de una lista de eventos
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

        static void VerPerAvDesactivado(List<EventRecord> LE, long IdActivacion, long IdDesactivacion)
        {
            Periodo PerDes = null;
            List<Periodo> LisPerDes = new List<Periodo>(); // se crea una lista de periodos de desactivación
            foreach (EventRecord E in LE) // para cada evento
            {
                if (E.Id != IdActivacion && E.Id != IdDesactivacion) continue; // se comprueba si el evento es de interés

                if (PerDes != null && E.Id == IdActivacion) // si es un evento de activación y existe un periodo de desactivación:
                {
                    PerDes.Tfin = E.TimeCreated.GetValueOrDefault(); // GetValueOrDefault() porque TimeCreated es de tipo Optional
                    PerDes.Duracion = PerDes.Tfin - PerDes.Tini; // calcular periodo restando los timestamps
                    LisPerDes.Add(PerDes); // añadir a la lista de periodos de desactivación
                    PerDes = null; // volver al estado lógico "activado" para permitir un nuevo periodo
                }
                else if (PerDes == null && E.Id == IdDesactivacion) // si es un evento de desactivación y NO existe un periodo de deactivación:
                {
                    PerDes = new Periodo(); // generar un nuevo periodo
                    PerDes.Tini = E.TimeCreated.GetValueOrDefault(); // guardar el timestamp de desactivación para el cáluclo posterior
                }
                else // en cualquier otro caso, estamos en un estado lógico inválido.
                {
                    Console.WriteLine("Error inesperado");
                    Environment.Exit(1);
                }
            }

            for (int i = 0; i < LisPerDes.Count; i++) // para cada periodo de desactivación, se imprime su inicio, fin y duración:
            {
                Periodo Periodo = LisPerDes[i];
                Console.WriteLine($"Período {i+1} de desactivación desde {Periodo.Tini} hasta {Periodo.Tfin} y duración {Periodo.Duracion}");
            }
        }
    }

    internal class Periodo
    {
        public DateTime Tini = new DateTime();
        public DateTime Tfin = new DateTime();
        public TimeSpan Duracion = new TimeSpan();
    }
}
