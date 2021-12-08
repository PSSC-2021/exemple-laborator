using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    [AsChoice]
    public static partial class ProductPublishedEvent
    {
        public interface IProductPublishedEvent { }

        public record ProductPublishScucceededEvent : IProductPublishedEvent 
        {
            public string Csv{ get;}
            public DateTime PublishedDate { get; }

            internal ProductPublishScucceededEvent(string csv, DateTime publishedDate)
            {
                Csv = csv;
                PublishedDate = publishedDate;
            }
        }

        public record ProductPublishFaildEvent : IProductPublishedEvent 
        {
            public string Reason { get; }

            internal ProductPublishFaildEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
