using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Data.Models
{
    public class GradeDto
    {
        public int CommandId { get; set; }
        public int ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? Total { get; set; }
    }
}
