using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(HNProject.Startup))]

namespace HNProject
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            SwaggerConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
