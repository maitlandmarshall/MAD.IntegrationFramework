using Autofac;
using Autofac.Extensions.DependencyInjection;
using MAD.IntegrationFramework.Configuration;
using MAD.IntegrationFramework.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace MAD.IntegrationFramework.Http
{
    internal class DefaultWebHostFactory : IWebHostFactory
    {
        private readonly MIFConfig config;

        public DefaultWebHostFactory(MIFConfig config)
        {
            this.config = config;
        }

        public IWebHost CreateWebHost()
        {
            IWebHostBuilder builder = WebHost
                .CreateDefaultBuilder()
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>()
                .UseIISIntegration();

            if (this.config.BindingPort == 80 || this.config.BindingPort == 443)
            {
                builder.UseHttpSys(options =>
                {
                    options.UrlPrefixes.Add($"http://*:{this.config.BindingPort}/{this.config.BindingPath}");
                });
            }
            else
            {
                builder.UseKestrel(options =>
                {
                    options.ListenAnyIP(this.config.BindingPort);
                });
            }

            return builder.Build();
        }

        class Startup
        {
            private ILifetimeScope autofacContainer;

            internal static void ApplyJsonOptions(MvcJsonOptions opt)
            {
                opt.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }

            // Use this method to add services to the container.
            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                services.AddOptions();

                services.AddMvc(config =>
                {
                    config.RespectBrowserAcceptHeader = true;

                    XmlSerializerOutputFormatter xml = new XmlSerializerOutputFormatter();
                    xml.WriterSettings.Indent = true;

                    config.OutputFormatters.Add(xml);
                })
                .AddJsonOptions(opt =>
                {
                    ApplyJsonOptions(opt);
                })
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
                })
                .AddControllersAsServices()
                .AddApplicationPart(Assembly.GetEntryAssembly());

                services.Configure<RazorViewEngineOptions>(y =>
                {
                    y.ViewLocationFormats.Clear();
                    y.ViewLocationFormats.Add("/Http/Views/{1}/{0}.cshtml");
                    y.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");

                    y.FileProviders.Add(new EmbeddedFileProvider(Assembly.GetEntryAssembly()));
                    y.FileProviders.Add(new EmbeddedFileProvider(typeof(Startup).Assembly));
                });

                ContainerBuilder builder = new ContainerBuilder();
                builder.RegisterModule<LoggingModule>();
                builder.RegisterModule<ConfigModule>();

                builder.Populate(services);

                this.autofacContainer = builder.Build();

                return new AutofacServiceProvider(this.autofacContainer);
            }

            // Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app)
            {
                app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "config",
                        template: "{controller}/{action=Index}"
                    );
                });
            }
        }
    }
}
