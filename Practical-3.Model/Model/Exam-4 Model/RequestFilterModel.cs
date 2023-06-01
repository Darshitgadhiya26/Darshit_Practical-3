using Darshit_Practical_3_Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Practical_3.Model.Model.Order;

namespace Practical_3.Model.Model.Exam_4_Model
{
    public class RequestFilterModel
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string? customerSearch { get; set; }
        public string? isActive { get; set; }
        public string? status { get; set; }
        public string? product { get; set; }
    }
    
}

