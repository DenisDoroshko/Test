using System.Diagnostics;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Web.Hosting;

namespace ThirdPartyEventEditor.Filters
{
    public class LogTimeFilter : FilterAttribute, IActionFilter
    {
        private const string _responseTimeKey = "ResponseTimeKey";
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Items[_responseTimeKey] = Stopwatch.StartNew();
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Stopwatch stopwatch = (Stopwatch)filterContext.HttpContext.Items[_responseTimeKey];
            var timeElapsed = stopwatch.Elapsed;
            var filePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath,
                    ConfigurationManager.AppSettings["loggingFilePath"]);
            using (var sw = new StreamWriter(filePath,true))
            {
                sw.WriteLineAsync($"Controller:{filterContext.ActionDescriptor.ControllerDescriptor.ControllerName}," +
                    $" Action:{filterContext.ActionDescriptor.ActionName},Time:{timeElapsed.TotalMilliseconds}ms");
            }
        }
    }
}