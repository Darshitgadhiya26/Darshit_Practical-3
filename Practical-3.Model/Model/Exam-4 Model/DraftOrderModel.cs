using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical_3.Model.Model.Exam_4_Model
{
    public class DraftOrderModel
    {
        public string Note { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerContactNo { get; set; }

        public AddressModel Address { get; set; }
        public ICollection<OrderItemModel> OrderItems { get; set; }



    }

    public class OrderItemModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public double Price { get; set; }
    }
}
