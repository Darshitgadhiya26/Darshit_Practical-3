using System.ComponentModel.DataAnnotations;

namespace Darshit_Practical_3_Web.Model
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
        public StatusType Status { get; set; }
        [Required]
        public double TotalAmount { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
        [Required]
        public string CustomerContactNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime ModifiedOn { get; set; } = DateTime.Now;

        public IEnumerable<OrderItems> orderItems { get; set; }
    }
}
