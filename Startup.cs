using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DAW_social_platform.Startup))]
namespace DAW_social_platform
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
