using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Autofac.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace MAD.IntegrationFramework.Http
{
    internal class Startup
    {
        internal static void ApplyJsonOptions(MvcJsonOptions opt)
        {
            opt.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
            .AddApplicationPart(Assembly.GetEntryAssembly()).AddControllersAsServices();

            services.Configure<RazorViewEngineOptions>(y =>
            {
                y.ViewLocationFormats.Clear();
                y.ViewLocationFormats.Add("/Http/Views/{1}/{0}.cshtml");
                y.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");

                y.FileProviders.Add(new EmbeddedFileProvider(typeof(Startup).Assembly));
                y.FileProviders.Add(new EmbeddedFileProvider(Assembly.GetEntryAssembly()));
            });
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //string bindingPath = MIF.Config.BindingPath;

            //if (!String.IsNullOrEmpty(bindingPath))
            //{
            //    if (!bindingPath.StartsWith("/"))
            //        bindingPath = $"/{bindingPath}";

            //    app.UsePathBase(bindingPath);
            //}

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
