using CosDeCumparaturi.Domain;
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
        public IEventPlatesteCos Execute(CommandPlateste command, Func<CodProdus, bool> checkProduct)
        {
            CosGol cosGol = new CosGol(command.InputCos);
            IStariCos cos = ValideazaCos(checkProduct, cosGol);
            cos = CalculateFinalGrades(cos);
            cos = PublishExamGrades(cos);

            return cos.Match(
                    whenCosGol: cosGol => new EventPlatesteCosFaild("Unexpected unvalidated state") as IEventPlatesteCos,
                    whenCosNevalidat: cosNevalidat => new EventPlatesteCosFaild(cosNevalidat.Reason),
                    whenCosValidat: cosValidat => new EventPlatesteCosFaild("Unexpected validated state"),
                    whenCosCalculat: CosCalculat => new EventPlatesteCosFaild("Unexpected calculated state"),
                    whenCosPlatit: cosPlatit => new EventPlatesteCosSuccess(cosPlatit.Csv, cosPlatit.PublishedDate)
                );
        }
    }
}
