using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.MicroKernel.Registration;

namespace MuleTest.NET.HttpServerMock
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            var castlecontrollerActivator = new CastleControllerActivator(container);
            container.Register(Component.For<CastleControllerActivator>().Instance(castlecontrollerActivator));
            
            var dynamicControllerSelector = new DynamicControllerSelector(container.Resolve<HttpConfiguration>());
            container.Register(Component.For<DynamicControllerSelector>().Instance(dynamicControllerSelector));

            var dynamicApiControllerActionSelector = new DynamicApiControllerActionSelector();
            container.Register(Component.For<DynamicApiControllerActionSelector>().Instance(dynamicApiControllerActionSelector));
        }
    }
}
