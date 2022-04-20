using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ThirdPartyEventEditor.App_Start.Startup))]
namespace ThirdPartyEventEditor.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}