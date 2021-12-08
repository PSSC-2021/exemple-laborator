using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Exemple.Domain.Models;

namespace Example.Api.Models
{
    public class InputProduct
    {
        [Required]
        [RegularExpression(CartCod.Pattern)]
        public string Cod { get; set; }

        [Required]
        [Range(1, 10)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 10)]
        public decimal Cantity { get; set; }
    }
}
