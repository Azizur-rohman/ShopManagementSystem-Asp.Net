using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Shop_Management_System.StartupOwin))]

namespace Shop_Management_System
{
    public partial class StartupOwin
    {
        public void Configuration(IAppBuilder app)
        {
            //AuthStartup.ConfigureAuth(app);
        }
    }
}
