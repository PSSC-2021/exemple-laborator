using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    [AsChoice]
    public static partial class ExamGradesPublishedEvent
    {
        public interface IExamGradesPublishedEvent { }

        public record ExamGradesPublishScucceededEvent : IExamGradesPublishedEvent 
        {
            public string Csv{ get;}
            public DateTime PublishedDate { get; }

            internal ExamGradesPublishScucceededEvent(string csv, DateTime publishedDate)
            {
                Csv = csv;
                PublishedDate = publishedDate;
            }
        }

        public record ExamGradesPublishFaildEvent : IExamGradesPublishedEvent 
        {
            public string Reason { get; }

            internal ExamGradesPublishFaildEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
