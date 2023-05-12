using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darshit_Practical_3_Web.Model
{
    public class OrderItems
    {
        [Key]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; }
        public Order Order { get; set; }
    }

}
