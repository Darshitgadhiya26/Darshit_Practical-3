using Darshit_Practical_3_Web.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Practical_3.Model.Model.Exam_4_Model;
using Practical_3.Model.Model;

namespace Final_Exam.Data
{
    public class ApplicationDBcontext : DbContext
    {


        public ApplicationDBcontext(DbContextOptions<ApplicationDBcontext> options) : base(options)
        {

        }

        public DbSet<AddressModel> Address { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderAddress> OrderAddress { get; set; }

        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Status> Statuses { get; set; }


    }
}
