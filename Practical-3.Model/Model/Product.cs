using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Darshit_Practical_3_Web.Model
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

        public double Price { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Quantity { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }
    }
}

