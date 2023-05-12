using System.ComponentModel.DataAnnotations;

namespace Darshit_Practical_3_Web.Model
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
    }
}
