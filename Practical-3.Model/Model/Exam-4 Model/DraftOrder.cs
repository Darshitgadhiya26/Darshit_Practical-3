using Darshit_Practical_3_Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Practical_3.Model.Model.Exam_4_Model
{
    public class DraftOrder
    {
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public List<OrderItems> Items { get; set; }

    }
}