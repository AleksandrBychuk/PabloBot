using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PabloBot.Services.Models;
using PabloBot.Services.Models.Profiles.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PabloBot
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PabloContext>(options =>
            {
                options.UseNpgsql("Host=localhost;Port=5432;Database=pablodb;Username=postgres;Password=postgres");
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddScoped<IProfileService, ProfileService>();

            var serviceProvider = services.BuildServiceProvider();

            var bot = new PabloBot(serviceProvider);
            services.AddSingleton(bot);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
