using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Data.Models
{
    public class GradeDto
    {
        public int GradeId { get; set; }
        public int StudentId { get; set; }
        public decimal? Exam { get; set; }
        public decimal? Activity { get; set; }
        public decimal? Final { get; set; }
    }
}
