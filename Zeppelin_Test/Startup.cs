using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Zeppelin_Test.Startup))]
namespace Zeppelin_Test
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
