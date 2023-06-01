using Darshit_Practical_3_Web.Model;
using Practical_3.Model.Model.Exam_4_Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical_3.Model.Model
{
    
        public class Order
        {
            [Key]
            public int OrderId { get; set; }

            public DateTime OrderDate { get; set; } = DateTime.Now.Date;
            public string Note { get; set; }
            public double DiscountAmount { get; set; }

            public enum StatusType
            {
                Open,
                Draft,
                Shipped,
                Paid
            }

            [Required]
            public StatusType Status { get; set; }

         
            public double TotalAmount { get; set; }

            [Required]
            public string CustomerName { get; set; }

            [Required]
            public string CustomerEmail { get; set; }

            public string CustomerContactNo { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedOn { get; set; } = DateTime.Now;
            public DateTime ModifiedOn { get; set; } = DateTime.Now;

            public ICollection<OrderItems> OrderItems { get; set; }


       
    }
    
}

