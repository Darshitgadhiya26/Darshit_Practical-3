using Darshit_Practical_3_Web.Model;
using Practical_3.Model.Model.Exam_4_Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Practical_3.Model.Model.VM
{
    public class OrderVm
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerContactNo { get; set; }
        public string Note { get; set; }
        public double DisountAmount { get; set; }
        public ICollection<OrderItems> orderItems { get; set; }


        public int AddressId { get; set; }
    }
}

