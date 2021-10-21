using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSSC.Domain
{
    [AsChoice]
    public static partial class ShoppingCartsPaidEvent
    {
        public interface IShoppingCartsPaidEvent { }

        public record ShoppingCartsPaidScucceededEvent : IShoppingCartsPaidEvent
        {
            public string Csv { get; }
            public DateTime PublishedDate { get; }

            internal ShoppingCartsPaidScucceededEvent(string csv, DateTime publishedDate)
            {
                Csv = csv;
                PublishedDate = publishedDate;
            }
        }

        public record ShoppingCartsPaidFailedEvent : IShoppingCartsPaidEvent
        {
            public string Reason { get; }

            internal ShoppingCartsPaidFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
