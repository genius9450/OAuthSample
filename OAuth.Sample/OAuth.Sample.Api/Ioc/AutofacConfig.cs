using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OAuth.Sample.Api.Ioc
{
    public class AutofacConfig
    {
        /// <summary>
        /// 連線字串
        /// </summary>
        public string DBConnectionString { get; set; }

        public void ConfigContainer(ContainerBuilder builder)
        {

            List<Assembly> assembly = new List<Assembly>();
            foreach (var item in System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory))
            {
                // 找當前資料夾裡的 dll 檔加入組件
                if ((System.IO.Path.GetExtension(item) == ".dll") && System.IO.Path.GetFileName(item).StartsWith("OAuth.Sample"))
                {
                    assembly.Add(Assembly.LoadFrom(item));
                }
            }

            // Controller 注入實體
            builder.RegisterAssemblyTypes(assembly.ToArray())
                .Where(t =>
                    t.Name.EndsWith("Controller") ||
                    t.Name.EndsWith("Process")
                )
                .PropertiesAutowired()          // 注入屬性				
                .InstancePerDependency();       // 每次呼叫建立唯一的實體(預設)

            // 找出所有 Service / Repository 並注入實體
            builder.RegisterAssemblyTypes(assembly.ToArray())
                .Where(t =>
                    t.Name.EndsWith("Service")
                )
                .WithProperty("DefaultConnectionString", DBConnectionString)
                .AsImplementedInterfaces()      // 以接口注入
                .PropertiesAutowired()          // 注入屬性		
                .EnableClassInterceptors()      // 啟用接口攔截，如不是使用 interface 攔截則要加上 virtual 才有作用
                .InstancePerDependency();       // 每次呼叫建立唯一的實體(預設)

            // 注入automapper
            //builder.RegisterType<MappingProfile>().As<GetProfileAsync>();
            builder.RegisterAssemblyTypes(assembly.ToArray())
                .Where(t =>
                    t.Name.EndsWith("GetProfileAsync")
                ).As<Profile>();

            builder.Register(c => new MapperConfiguration(cfg =>
            {
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    cfg.AddProfile(profile);
                }
            })).AsSelf().SingleInstance();
            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve)).As<IMapper>().InstancePerLifetimeScope();

            // 找出所有 Context 並注入實體
            builder.RegisterAssemblyTypes(assembly.ToArray())
                .Where(t =>
                    t.Name.EndsWith("Context")
                )
                .AsSelf();

        }
    }
}

