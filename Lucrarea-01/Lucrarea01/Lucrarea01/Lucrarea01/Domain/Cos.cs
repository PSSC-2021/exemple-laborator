using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucrarea01.Domain
{
    [AsChoice]
    public static partial class Cos
    {
        public interface ICos
        {   }

        public record UnvalidatedCos(IReadOnlyCollection<UnvalidatedProduse> ProduseList, CosDetails CosDetails) : ICos;

        public record InvalidatedCos(IReadOnlyCollection<UnvalidatedProduse> ProduseList, string reason) : ICos;

        public record GolCos(IReadOnlyCollection<UnvalidatedProduse> ProduseList, string reason) : ICos;
        
        public record ValidatedCos(IReadOnlyCollection<ValidatedProduse> ProduseList, CosDetails CosDetails) : ICos;

        public record CosPlatit(IReadOnlyCollection<ValidatedProduse> ProduseList, CosDetails CosDetails, DateTime PublishedDate) : ICos;

    }
}
