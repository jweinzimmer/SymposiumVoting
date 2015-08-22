using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SymposiumVoting.Startup))]
namespace SymposiumVoting
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
