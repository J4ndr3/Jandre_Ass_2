using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Ass2.Startup))]
namespace Ass2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
