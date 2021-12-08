using CosDeCumparaturi.Domain;
using System;
using System.Collections.Generic;
using static CosDeCumparaturi.Domain.StariCos;

namespace Exemple
{
    class Program
    {
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            var listaCosuri = citesteLista().ToArray();
            CosGol cosNevalidat = new(listaCosuri);
            IStariCos result = ValideazaCos(cosNevalidat);
            result.Match(
                whenCosGol: unvalidatedResult => cosNevalidat,
                whenCosPlatit: publishedResult => publishedResult,
                whenCosNevalidat: invalidResult => invalidResult,
                whenCosValidat: validatedResult => PublishExamGrades(validatedResult)
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

                CodProdus obCod = new CodProdus(Int32.Parse(cod));
                Cantitate obCantitate = new Cantitate(Int32.Parse(cantitate));
                Adresa obAdresa = new Adresa(adresa);

                listaCosuri.Add(new (obCod, obCantitate, obAdresa));
            } while (true);
            return listaCosuri;
        }

        private static IStariCos ValideazaCos(CosGol cosInvalid) =>
            random.Next(100) > 50 ?
            new CosNevalidat(new List<StareInvalid>(), "Random errror")
            : new CosValidat(new List<StareValid>());

        private static IStariCos PublishExamGrades(CosValidat cosValidat) =>
            new CosPlatit(new List<StareValid>(), DateTime.Now);

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
