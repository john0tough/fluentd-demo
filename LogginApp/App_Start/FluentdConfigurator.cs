/*
 *docker pull fluent/fluentd:latest
Descarga la imagen docker de fluentd

# /tmp/fluentd.conf
<source>
  @type http
  port 9880
  bind 0.0.0.0
</source>
<match **>
  @type stdout
</match>
Crear archivo fluentd.conf, el cual especifica que va a recibir trafico por el puesto 9880

docker run -d -p 9880:9880 -v b:/:/fluentd/etc -e FLUENTD_CONF=fluentd.conf fluent/fluentd:latest
Inicia el servicio en el puerto especificado, deacuerdo a 
-d debug
-p puerto salida:entrada
-v monta volumen  ruta/windows:/ruta/servicio
-e especifica config
fluent/fluentd:latest especifica el servicio a iniciar

 Invoke-WebRequest -Uri http://localhost:98
80/sample.test -Method POST -Body @{json='{"a":"b"}'}

Peticion a servicio log por POST en powershell

http://localhost:9880/sample.test?json={"me canso":"ganzo"}

 *configurar para capturar streams
 * config
 * establece el tipo stream de datos
 <source>
  @type forward
  port 24224
</source>
match establece cuales tags se van a captutrar, en este caso seran todos.
<match **>
  @type stdout
</match>
 *
 inicia servicio en docker
 * docker run -d -p 24224:24224 -p 24224:24224/udp -v b:/:/fluentd/etc -e FLUENTD_CONF=fluentd.conf fluent/fluentd:latest
 *
 *
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using NLog;
using NLog.Targets.Wrappers;

namespace LogginApp.App_Start
{
   public class FluentdConfigurator
   {
      //configuracion de fluentd con .net 
      // https://qiita.com/akehoyayoi/items/87b268327dd2c2b6cd73
      // archivo de configuracion
      // https://docs.fluentd.org/v0.12/articles/config-file
      static NLog.Targets.Target GetFluentdTarget()
      {
         var target = new NLog.Targets.Fluentd();
         //revisar https://github.com/NLog/NLog/wiki/JsonLayout
         // target.Layout = new NLog.Layouts.JsonLayout();
         target.Host = "127.0.0.1";
         target.Port = 24224;
         target.Tag = "net.fluentd";
         // https://github.com/NLog/NLog/wiki/Layouts
         target.Layout = new NLog.Layouts.SimpleLayout("${longdate}|${level}|${callsite}|${logger}|${message}");
         target.NoDelay = true;
         target.LingerEnabled = false;
         target.LingerTime = 2;
         target.EmitStackTraceWhenAvailable = false;
         return WrapTarget(target);
      }

      static NLog.Targets.Target WrapTarget(NLog.Targets.Target target)
      {
         var retryWrapper = new NLog.Targets.Wrappers.RetryingTargetWrapper("RetryningWrapper", target, 3, 1000);
         var asyncWrapper = new NLog.Targets.Wrappers.AsyncTargetWrapper("AsyncWrapper", retryWrapper);
         return asyncWrapper;
      }

      public static NLog.Config.LoggingConfiguration GetConfig()
      {
         var config = new NLog.Config.LoggingConfiguration();
         var target = GetFluentdTarget();
         config.AddTarget("fluentd", target);
         // el parametro LoggerNamePattern, se usa para extraer el logger de fluentd en donde se requiera
         // el nivel de log establece nivel de log maximo que se va a capturar, e.j. un LogLevel.Debug no capturara los logs de tipo LogLevel.Trace
         config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Debug, target));
         return config;
      }
   }
}