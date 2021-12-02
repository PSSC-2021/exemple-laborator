using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Data.Models
{
    public class ProductDto
    {
        public int ProductCod { get; set; }
        public int CartCod { get; set; }
        public decimal? Price { get; set; }
        public decimal? Cantity { get; set; }
        public decimal? Total { get; set; }
    }
}
