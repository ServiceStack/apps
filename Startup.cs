using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Script;
using ServiceStack.Web;
using System;
using System.IO;
using Apps.ServiceInterface;
using ServiceStack.IO;
using ServiceStack.Text;
using ServiceStack.Logging;

namespace Apps
{
    public class Startup : ModularStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public new void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(new Sites());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost
            {
                AppSettings = new NetCoreAppSettings(Configuration)
            });
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("ServiceStack Apps", typeof(AppServices).Assembly) { }

        // Configure your AppHost with the necessary configuration and dependencies your App needs
        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                UseSameSiteCookies = true,
                DebugMode = HostingEnvironment.IsDevelopment()
            });
            
            Plugins.Add(new CorsFeature(allowOriginWhitelist:new[] {
                "https://localhost:5001",
                "https://gist.cafe",
            }));
            
            Plugins.Add(new SharpPagesFeature {
            });
        }
    }
}
