using Darshit_Practical_3_Web.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical_3.Model.Model.Exam_4_Model
{

    public class OrderAddress
        {
            [Key]
            public int OrderAddId { get; set; }
            public int OrderId { get; set; }
            public int AddressId { get; set; }
            public  Order Order { get; set; }
            public AddressModel Address { get; set; }

    }
    
}
