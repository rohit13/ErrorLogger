using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ErrorLogger.Startup))]
namespace ErrorLogger
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
