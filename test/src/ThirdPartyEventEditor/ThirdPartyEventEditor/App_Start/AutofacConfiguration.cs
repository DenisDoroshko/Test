using Autofac;
using Autofac.Integration.Mvc;
using System.Web.Mvc;
using ThirdPartyEventEditor.Data;
using ThirdPartyEventEditor.Services;
using ThirdPartyEventEditor.Models;
using System.Configuration;
using System.Web.Hosting;
using System.IO;

namespace ThirdPartyEventEditor.App_Start
{
    public class AutofacConfiguration
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(AutofacConfiguration).Assembly);
            builder.RegisterType<EventRepository>().As<IRepository<ThirdPartyEvent>>()
                .WithParameter("sourcePath", Path.Combine(HostingEnvironment.ApplicationPhysicalPath,
                ConfigurationManager.AppSettings["EventsFilePath"]));
            builder.RegisterType<EventService>().As<IEventService>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}