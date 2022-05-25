using DataAccessLogic;
using DataAccessLogic.CrudLogic;
using DataAccessLogic.DatabaseModels;
using DataAccessLogic.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechSaleTelegramBot;
using WebApplicationTechSale.HelperServices;
using Microsoft.Extensions.Logging.AzureAppServices;

namespace WebApplicationTechSale
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserValidator<User>, UserValidator>();
            services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(Configuration["RemoteDatabase"]));
            services.AddTransient<ICrudLogic<Antiques>, AntiquesLogic>();
            services.AddTransient<ICrudLogic<User>, UserLogic>();
            services.AddTransient<ICrudLogic<Note>, NoteLogic>();
            services.AddTransient<ISavedLogic, SavedListLogic>();
            services.AddTransient<IPagination<Antiques>, AntiquesLogic>();
            services.AddSingleton<IBot, TechSaleBot>(bot => new TechSaleBot(Configuration["BotToken"]));
            services.AddHostedService<BotHostService>();
            services.AddTransient<ICrudLogic<Order>, OrderLogic>();

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<ApplicationContext>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Antiques}/{id?}");
            });
        }
    }
}