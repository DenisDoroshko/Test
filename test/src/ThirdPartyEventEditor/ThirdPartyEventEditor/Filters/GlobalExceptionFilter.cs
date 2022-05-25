using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Web.Hosting;

namespace ThirdPartyEventEditor.Filters
{
    public class GlobalExceptionFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext exceptionContext)
        {
            if (!exceptionContext.ExceptionHandled)
            {
                exceptionContext.Result = new RedirectResult($"/Home/Error?message=" +
                    $"{exceptionContext.Exception.Message.Replace("\r\n","")}");
                exceptionContext.ExceptionHandled = true;
                var filePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath,
                    ConfigurationManager.AppSettings["loggingFilePath"]);
                using (var sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLineAsync($"Exception:{exceptionContext.Exception.GetType().Name}," +
                        $" Message:{exceptionContext.Exception.Message}");
                }
            }
        }
    }
}