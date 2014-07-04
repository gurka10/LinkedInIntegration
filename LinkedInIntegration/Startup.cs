using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LinkedInIntegration.Startup))]
namespace LinkedInIntegration
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
