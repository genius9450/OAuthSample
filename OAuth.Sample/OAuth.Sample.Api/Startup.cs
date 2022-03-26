using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OAuth.Sample.Api.Attribute;
using OAuth.Sample.Api.Middleware;
using OAuth.Sample.Api.Ioc;
using Autofac;
using OAuth.Sample.Api;
using OAuth.Sample.Domain.Shared;
using Microsoft.Extensions.Logging;
using OAuth.Sample.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace OAuth.Sample.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
                               .SetBasePath(environment.ContentRootPath)
                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                               .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                               .AddEnvironmentVariables();

            Configuration = builder.Build();

            #region ��l�ưѼ�

            Const.EnvironmentName = environment.EnvironmentName;
            Const.DefaultConnectionString = Configuration["DBConnectionString:DefaultConnectionString"];

            #endregion            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddControllersAsServices()                   
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddMvc(config =>
            {
                config.Filters.Add(new TypeFilterAttribute(typeof(ActionLogAttribute)));
                config.Filters.Add(new TypeFilterAttribute(typeof(ModelStateValidationAttribute)));
            });

            //Seq Logger
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSeq(Configuration.GetSection("Seq"));
            });

            // runtime db
            services.AddDbContext<OAuthSampleDBContext>(options =>
            {
                options.UseSqlServer(Const.DefaultConnectionString);
            });

            // Register Swagger services
            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "OAuth.Sample", Version = "v1" });
            });

            services.AddHttpClient();

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //AutoFac Ioc
            var config = new AutofacConfig
            {
                DBConnectionString = Const.DefaultConnectionString
            };
            config.ConfigContainer(builder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Debug"))
            {
                app.UseDeveloperExceptionPage();                

                //app.UseHttpsRedirection();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

                //app.UseHttpsRedirection();
            }

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseSwagger();
            app.UseSwaggerUI(c=> 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "OAuth.Sample");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");  // ����

            app.UseMiddleware<ExceptionMiddleware>(); // ��������Exception

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/{action}"
                );
            });

            Const.Logger = logger;
        }
    }
}

