using CosDeCumparaturi;
using CosDeCumparaturi.Domain;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CosDeCumparaturi.Domain.StariCos;

namespace Exemple
{
    class Program
    {
        private static readonly Random random = new Random();

        static async Task Main(string[] args)
        {
            var listaCosuri = citesteLista().ToArray();
            CommandPlateste command = new(listaCosuri);
            PlatesteCosWorkflow workflow = new PlatesteCosWorkflow();
            var result = await workflow.Execute(command, CheckCosExists);


            result.Match(
                    whenEventPlatesteCosFailed: @event =>
                    {
                        Console.WriteLine($"Publish failed: {@event.Reason}");
                        return @event;
                    },
                    whenEventPlatesteCosSuccess: @event =>
                    {
                        Console.WriteLine($"Publish succeeded.");
                        Console.WriteLine(@event.Csv);
                        return @event;
                    }
                );

            Console.WriteLine("Hello World!");
        }

        private static List<StareInvalid> citesteLista()
        {
            List <StareInvalid> listaCosuri = new();
            do
            {
                //read registration number and grade and create a list of greads
                var cod = ReadValue("Cod Produs: ");
                if (string.IsNullOrEmpty(cod))
                {
                    break;
                }

                var cantitate = ReadValue("Cantitate: ");
                if (string.IsNullOrEmpty(cantitate))
                {
                    break;
                }

                var adresa = ReadValue("Adresa: ");
                if (string.IsNullOrEmpty(adresa))
                {
                    break;
                }

                var pret = ReadValue("Pret: ");
                if (string.IsNullOrEmpty(pret))
                {
                    break;
                }

                CodProdus obCod = new CodProdus(Int32.Parse(cod));
                Cantitate obCantitate = new Cantitate(Int32.Parse(cantitate));
                Adresa obAdresa = new Adresa(adresa);
                Pret obPret = new Pret(Int32.Parse(pret));

                listaCosuri.Add(new (obCod, obCantitate, obAdresa, obPret));
            } while (true);
            return listaCosuri;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        private static TryAsync<bool> CheckCosExists(CodProdus cod) => async () => true;
    }
}
