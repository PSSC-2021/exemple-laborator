using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosDeCumparaturi.Domain
{
    [AsChoice]
    public static partial class StariCos
    {
        public interface IStariCos { }


        public record CosGol : IStariCos
        {
            public CosGol(IReadOnlyCollection<StareInvalid> listaCos)
            {
                StareList = listaCos;
            }

            public IReadOnlyCollection<StareInvalid> StareList { get; }
        }


        public record CosNevalidat : IStariCos
        {
            internal CosNevalidat(IReadOnlyCollection<StareInvalid> listaCos, string reason)
            {
                ListaCos = listaCos;
                Reason = reason;
            }

            public IReadOnlyCollection<StareInvalid> ListaCos { get; }
            public string Reason { get; }
        }


        public record CosValidat : IStariCos
        {
            internal CosValidat(IReadOnlyCollection<StareValid> listaCos)
            {
                ListaCos = listaCos;
            }

            public IReadOnlyCollection<StareValid> ListaCos { get; }
        }



        public record CosCalculat : IStariCos
        {
            internal CosCalculat(IReadOnlyCollection<StareCalculat> listaCos)
            {
                ListaCos = listaCos;
            }

            public IReadOnlyCollection<StareCalculat> ListaCos { get; }
        }


        public record CosPlatit : IStariCos
        {
            internal CosPlatit(IReadOnlyCollection<StareCalculat> listaCos, string csv, DateTime publishedDate)
            {
                ListaCos = listaCos;
                PublishedDate = publishedDate;
                Csv = csv;
            }

            public IReadOnlyCollection<StareCalculat> ListaCos { get; }
            public DateTime PublishedDate { get; }
            public string Csv { get; }
        }
    }
}
