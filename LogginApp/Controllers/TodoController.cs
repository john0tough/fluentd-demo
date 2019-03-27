using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogginApp.App_Start;
using NLog;
using LogLevel = NLog.LogLevel;

namespace LogginApp.Controllers
{
   [RoutePrefix("api/todo")]
   public class TodoController : ApiController
   {
      [HttpGet]
      public IHttpActionResult GetAll()
      {
         //el logfactory se debe usar cuando se establecen diferentes tipos de configuracion, por ejemplo diferentes servicios de log, ya que se le pasa la configuracion del tipo y no toma el tipo por defecto
         // var loggerFactory = new LogFactory(FluentdConfigurator.GetConfig());
         // loggerFactory.GetLogger("*");

         var logger = LogManager.GetLogger("*");
         var target = logger.Factory.Configuration.FindTargetByName<NLog.Targets.Fluentd>("*");
         target.Tag = "tag.uno";
         logger.Log(LogLevel.Debug, Request);
         var response = "esta es otra respuesta de peticion";
         logger.Factory.Configuration.FindTargetByName<NLog.Targets.Fluentd>("*").Tag = "tag.dos";
         logger.Log(LogLevel.Debug, new
         {
            response = response
         });
         return Ok(response);
      }
   }
}
