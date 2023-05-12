using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical_3.Model.Model.VM
{
    public class OrdersVM
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerContactNo { get; set; }
        public string Note { get; set; }
        public ICollection<OrderItemVM> OrderItems { get; set; }
    }
}
