using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CosDeCumparaturi.Domain.StariCos;

namespace CosDeCumparaturi.Domain
{
    public record CommandPlateste
    {
        public CommandPlateste(IReadOnlyCollection<StareInvalid> inputCos)
        {
            InputCos = inputCos;
        }

        public IReadOnlyCollection<StareInvalid> InputCos { get; }
    }

}
