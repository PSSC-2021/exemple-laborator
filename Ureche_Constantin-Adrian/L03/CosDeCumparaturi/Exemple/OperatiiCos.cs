using CosDeCumparaturi.Domain;
using static LanguageExt.Prelude;
using LanguageExt;
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
        public static Task<IStariCos> ValideazaCos(Func<CodProdus, TryAsync<bool>> checkCos, CosGol cos) =>
            cos.StareList
                      .Select(Valideaza(checkCos))
                      .Aggregate(CreateEmptyValatedCosList().ToAsync(), ReduceValidCos)
                      .MatchAsync(
                            Right: cosValidat => new CosValidat(cosValidat),
                            LeftAsync: errorMessage => Task.FromResult((IStariCos)new CosNevalidat(cos.StareList, errorMessage))
                      );

        private static Func<StareInvalid, EitherAsync<string, StareValid>> Valideaza(Func<CodProdus, TryAsync<bool>> checkCos) =>
            stareInvalid => Valideaza(checkCos, stareInvalid);

        private static EitherAsync<string, StareValid> Valideaza(Func<CodProdus, TryAsync<bool>> checkCos, StareInvalid nevalidat)=>
            from cantitate in Cantitate.TryParse(nevalidat.cantitate.ToString())
                                   .ToEitherAsync(() => $"Invalid  ({nevalidat.cod}, {nevalidat.cantitate})")
            from codProdus in CodProdus.TryParse(nevalidat.cod.ToString())
                                   .ToEitherAsync(() => $"Invalid  ({nevalidat.cod}, {nevalidat.cod})")
            from pret in Pret.TryParse(nevalidat.pret.ToString())
                                   .ToEitherAsync(() => $"Invalid  ({nevalidat.cod}, {nevalidat.pret})")
            from adresa in Adresa.TryParse(nevalidat.pret.ToString())
                                   .ToEitherAsync(() => $"Invalid  ({nevalidat.cod}, {nevalidat.adresa})")
            from cosExists in checkCos(codProdus)
                                   .ToEither(error => error.ToString())
            select new StareValid(codProdus, cantitate, adresa, pret);

        private static Either<string, List<StareValid>> CreateEmptyValatedCosList() =>
            Right(new List<StareValid>());

        private static EitherAsync<string, List<StareValid>> ReduceValidCos(EitherAsync<string, List<StareValid>> acc, EitherAsync<string, StareValid> next) =>
            from list in acc
            from nextGrade in next
            select list.AppendValidCos(nextGrade);

        private static List<StareValid> AppendValidCos(this List<StareValid> list, StareValid validGrade)
        {
            list.Add(validGrade);
            return list;
        }

        public static IStariCos CalculateFinalCos(IStariCos examGrades) => examGrades.Match(
            whenCosGol: cosGol => cosGol,
            whenCosNevalidat: cosNevalidat => cosNevalidat,
            whenCosCalculat: CosCalculat => CosCalculat,
            whenCosPlatit: cosPlatit => cosPlatit,
            whenCosValidat: CalculeazaFinalCos
        );

        private static IStariCos CalculeazaFinalCos(CosValidat valid) =>
            new CosCalculat(valid.ListaCos
                                        .Select(CalculateFinal)
                                        .ToList()
                                        .AsReadOnly());

        private static StareCalculat CalculateFinal(StareValid valid) => 
            new StareCalculat(valid.cod,
                                      valid.cantitate,
                                      valid.adresa,
                                      valid.pret * valid.cantitate);

        public static IStariCos platesteCos(IStariCos cos) => cos.Match(
            whenCosGol: cosGol => cosGol,
            whenCosNevalidat: cosNevalidat => cosNevalidat,
            whenCosValidat: cosValid => cosValid,
            whenCosPlatit: cosPlatit => cosPlatit,
            whenCosCalculat: GenerateExport);

        private static IStariCos GenerateExport(CosCalculat calculat) => 
            new CosPlatit(calculat.ListaCos, 
                                    calculat.ListaCos.Aggregate(new StringBuilder(), CreateCsvLine).ToString(), 
                                    DateTime.Now);

        private static StringBuilder CreateCsvLine(StringBuilder export, StareCalculat grade) =>
            export.AppendLine($"{grade.cod.Value}, {grade.cantitate}, {grade.adresa}, {grade.pret}");
    }
}
