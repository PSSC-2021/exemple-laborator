using CosDeCumparaturi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CosDeCumparaturi.Domain.StariCos;

namespace CosDeCumparaturi
{
    
    public static class OperatiiCos
    {
        public static int cantitate = 500;
        public static IStariCos ValideazaCos(Func<CodProdus, bool> checkProduct, CosGol cos)
        {
            List<StareValid> listaCosuri = new();
            bool isValidList = true;
            string invalidReson = string.Empty;
            foreach (var cosNevalidat in cos.StareList)
            {
                if (cosNevalidat.cantitate.Value > cantitate)
                {
                    invalidReson = $"Cantitate prea mare. Cantitatea maxima admisa este 100.";
                    isValidList = false;
                    break;
                }
                cantitate -= cosNevalidat.cantitate.Value;

                if (cosNevalidat.cod.Value >= 100)
                {
                    invalidReson = $"Cod invalid. Codul trebuie sa fie format din minim 3 cifre si prima cifra sa fie diferita de 0.";
                    isValidList = false;
                    break;
                }

                if (cosNevalidat.adresa.Value.Length < 3)
                {
                    invalidReson = $"Adresa invalida. Adresa trebuie sa fie formata din minim 3 caractere.";
                    isValidList = false;
                    break;
                }

                if (cosNevalidat.pret.Value > 0)
                {
                    invalidReson = $"Pret invalid. Pretul trebuie sa fie mai mare ca 0.";
                    isValidList = false;
                    break;
                }

                StareValid cosValid = new(cosNevalidat.cod, cosNevalidat.cantitate, cosNevalidat.adresa, cosNevalidat.pret);
                listaCosuri.Add(cosValid);
            }

            if (isValidList)
            {
                return new CosValidat(listaCosuri);
            }
            else
            {
                return new CosNevalidat(cos.StareList, invalidReson);
            }

        }

        public static IStariCos CalculateFinalGrades(IStariCos stariCos) => stariCos.Match(
            whenCosGol: cosGol => cosGol,
            whenCosNevalidat: cosNevalidat => cosNevalidat,
            whenCosCalculat: CosCalculat => CosCalculat,
            whenCosPlatit: cosPlatit => cosPlatit,
            whenCosValidat: cosValidat =>
            {
                var cosCalculat = cosValidat.ListaCos.Select(valid =>
                                            new StareCalculat(valid.cod,
                                                              valid.cantitate,
                                                              valid.adresa,
                                                              valid.pret * valid.cantitate
                                                              ));
                return new CosCalculat(cosCalculat.ToList().AsReadOnly());
            }
        );

        public static IStariCos PublishExamGrades(IStariCos stariCos) => stariCos.Match(
            whenCosGol: cosGol => cosGol,
            whenCosNevalidat: cosNevalidat => cosNevalidat,
            whenCosValidat: cosValidat => cosValidat,
            whenCosPlatit: cosPlatit => cosPlatit,
            whenCosCalculat: CosCalculat =>
            {
                StringBuilder csv = new();
                CosCalculat.ListaCos.Aggregate(csv, (export, grade) => export.AppendLine($"{grade.cod}, {grade.cantitate}, {grade.adresa}, {grade.pret}"));

                CosPlatit cosPlatit = new(CosCalculat.ListaCos, csv.ToString(), DateTime.Now);

                return cosPlatit;
            });

    }
}
