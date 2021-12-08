using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosDeCumparaturi.Domain
{

    [AsChoice]
    public static partial class EventPlatesteCos
    {
        public interface IEventPlatesteCos { }

        public record EventPlatesteCosSuccess : IEventPlatesteCos
        {
            public string Csv { get; }
            public DateTime PublishedDate { get; }

            internal EventPlatesteCosSuccess (string csv, DateTime publishedDate)
            {
                Csv = csv;
                PublishedDate = publishedDate;
            }
        }

        public record EventPlatesteCosFaild : IEventPlatesteCos
        {
            public string Reason { get; }

            internal EventPlatesteCosFaild(string reason)
            {
                Reason = reason;
            }
        }
    }
}
