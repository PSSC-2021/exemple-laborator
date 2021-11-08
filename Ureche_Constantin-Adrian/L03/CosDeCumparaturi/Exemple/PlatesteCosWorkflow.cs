using CosDeCumparaturi.Domain;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CosDeCumparaturi.Domain.EventPlatesteCos;
using static CosDeCumparaturi.Domain.StariCos;
using static CosDeCumparaturi.OperatiiCos;

namespace CosDeCumparaturi
{
    class PlatesteCosWorkflow
    {
        public async Task<IEventPlatesteCos> Execute(CommandPlateste command, Func<CodProdus, TryAsync<bool>> checkProduct)
        {
            CosGol cosGol = new CosGol(command.InputCos);
            IStariCos cos = await ValideazaCos(checkProduct, cosGol);
            cos = CalculateFinalCos(cos);
            cos = platesteCos(cos);

            return cos.Match(
                    whenCosGol: cosGol => new EventPlatesteCosFailed("Unexpected unvalidated state") as IEventPlatesteCos,
                    whenCosNevalidat: cosNevalidat => new EventPlatesteCosFailed(cosNevalidat.Reason),
                    whenCosValidat: cosValidat => new EventPlatesteCosFailed("Unexpected validated state"),
                    whenCosCalculat: CosCalculat => new EventPlatesteCosFailed("Unexpected calculated state"),
                    whenCosPlatit: cosPlatit => new EventPlatesteCosSuccess(cosPlatit.Csv, cosPlatit.PublishedDate)
                );
        }
    }
}
