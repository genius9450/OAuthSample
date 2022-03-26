using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OAuth.Sample.Api.Ioc;
using OAuth.Sample.EF;
using OAuth.Sample.Schedule.Interface;
using Autofac;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OAuth.Sample.Api.Helper;
using Hangfire.SqlServer;

namespace OAuth.Sample.Schedule
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                 .SetBasePath(environment.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                 .AddEnvironmentVariables();

            Const.DefaultConnectionString = Configuration["DBConnectionString:DefaultConnectionString"];

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages().AddControllersAsServices();

            // runtime db
            services.AddDbContext<OAuthSampleDBContext>(options =>
            {
                options.UseSqlServer(Const.DefaultConnectionString);
            });

            // Seq Logger
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSeq(Configuration.GetSection("Seq"));
            });

            #region Hangfire

            // �ϥ�Application Memory
            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage());

            // �ϥ�Sql Server
            //services.AddHangfire(config =>
            //    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            //    .UseSimpleAssemblyNameTypeSerializer()
            //    .UseDefaultTypeSerializer()
            //    .UseSqlServerStorage(Const.DefaultConnectionString, new SqlServerStorageOptions
            //    {
            //        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //        QueuePollInterval = TimeSpan.Zero,
            //        UseRecommendedIsolationLevel = true,
            //        DisableGlobalLocks = true
            //    }));

            services.AddHangfireServer();

            #endregion            
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //AutoFac Ioc�`�J
            var config = new AutofacConfig();
            config.ConfigContainer(builder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // �ϥ�Hangfire Dashboard
            app.UseHangfireDashboard();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            // �}�l����Ƶ{
            //BackgroundJob.Enqueue<IScheduleService>(x => x.Start());
            BackgroundJob.Enqueue(() => Console.WriteLine("Startup: Hello world!"));
        }
    }
}

