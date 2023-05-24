using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Web.Mvc;

namespace KS.Web.Dependency
{
    public class ControllerDependencyConventions : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                                .BasedOn<IController>()
                                .LifestyleTransient());
            
        }

       
        //               .AddFacility<LoggingFacility>(f => f.UseLog4Net());

        //    LoggerFactory.SetCurrent(new TraceSourceLogFactory());
        //    EntityValidatorFactory.SetCurrent(new DataAnnotationsEntityValidatorFactory());

        //    var typeAdapterFactory = container.Resolve<ITypeAdapterFactory>();
        //    TypeAdapterFactory.SetCurrent(typeAdapterFactory);

        //}
    }
}