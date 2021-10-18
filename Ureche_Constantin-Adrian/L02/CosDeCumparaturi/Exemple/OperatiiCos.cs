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
        public static IStariCos ValideazaCos(Func<CodProdus, bool> checkProduct, CosGol cos)
        {
            List<StareValid> listaCosuri = new();
            bool isValidList = true;
            string invalidReson = string.Empty;
            foreach (var cosNevalidat in cos.StareList)
            {
                if (cosNevalidat.cantitate.Value < 100)
                {
                    invalidReson = $"Cantitate prea mare. Cantitatea maxima admisa este 100.";
                    isValidList = false;
                    break;
                }

                if (cosNevalidat.cod.Value > 100)
                {
                    invalidReson = $"Cod invalid. Codul trebuie sa fie format din minim 3 cifre si prima cifra sa fie diferita de 1.";
                    isValidList = false;
                    break;
                }

                StareValid cosValid = new(cosNevalidat.cod, cosNevalidat.cantitate, cosNevalidat.adresa);
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
                                                              valid.adresa
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
                CosCalculat.ListaCos.Aggregate(csv, (export, grade) => export.AppendLine($"{grade.cod}, {grade.cantitate}, {grade.adresa}"));

                CosPlatit cosPlatit = new(CosCalculat.ListaCos, csv.ToString(), DateTime.Now);

                return cosPlatit;
            });
    }
}
