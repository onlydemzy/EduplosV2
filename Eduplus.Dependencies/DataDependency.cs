using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using KS.Core;
using KS.Data;

namespace Eduplus.Dependencies
{
    public class DataDependency:IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IUnitOfWork, UnitOfWork>().ImplementedBy<UnitOfWork>().LifestyleTransient());
                //Component.For<IQueryableUnitOfWork>(),
                

           
        }
    }
}
