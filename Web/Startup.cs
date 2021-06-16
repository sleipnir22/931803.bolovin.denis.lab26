using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Data;

namespace Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("PostgreSQL");
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connection));

            services.AddControllersWithViews();
            
            services.AddAuthentication("Cookie")
                .AddCookie("Cookie", options =>
                {
                    options.Cookie.Name = "Cheburek";
                    options.LoginPath = "/Account/Login";
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(new StaticFileOptions {RequestPath = "/static"});
            
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=home}/{action=Index}");
            });
        }
    }
}