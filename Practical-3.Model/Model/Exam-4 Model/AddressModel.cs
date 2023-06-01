using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical_3.Model.Model.Exam_4_Model
{
    public enum AddressTypeEnum
    {
        Shipping,
        Billing
    }

    public class AddressModel
    {
        [Key]
        public int AddressId { get; set; }
        public AddressTypeEnum AddressType { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNo { get; set; }

    }


}
