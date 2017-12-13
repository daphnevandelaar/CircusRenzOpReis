using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CircusRenzOpReis.Startup))]
namespace CircusRenzOpReis
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
