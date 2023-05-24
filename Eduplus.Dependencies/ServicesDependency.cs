using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Eduplus.Services.Implementations;
using KS.Services.Implementation;
using System.Reflection;

namespace Eduplus.Dependencies
{
    public class ServicesDependency:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
             
                Classes.FromAssembly(Assembly.GetAssembly(typeof(UserService)))
                .InSameNamespaceAs<UserService>().WithService.DefaultInterfaces().LifestyleTransient(),

                Classes.FromAssembly(Assembly.GetAssembly(typeof(GeneralDutiesService)))
                .InSameNamespaceAs<GeneralDutiesService>().WithService.DefaultInterfaces().LifestyleTransient()
                
                );
        }
    }
}
