using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using LogginApp.App_Start;
using NLog;

namespace LogginApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           // Setea configuracion de fluentd como proveedor de servicio de logueo para nlog
           LogManager.Configuration = FluentdConfigurator.GetConfig();
           GlobalConfiguration.Configure(WebApiConfig.Register);

        }
    }
}
