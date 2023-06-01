
using Final_Exam.Data;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: FunctionsStartup(typeof(Final_Exam.Startup))]
namespace Final_Exam
{
    public class Startup : FunctionsStartup
    {
        private readonly IConfiguration _configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var con = "Server=localhost;User=root;Password=;Database=Darshit_practical4;Convert Zero DateTime=True";

            builder.Services.AddDbContext<ApplicationDBcontext>(options => options.UseMySql(con, ServerVersion.AutoDetect(con)));

        }


    }
}
