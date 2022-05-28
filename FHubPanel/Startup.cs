using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FHubPanel.Startup))]
namespace FHubPanel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
