using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace MuleTest.NET.HttpServerMock
{
    public class DynamicControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;

        private readonly List<Type> _controllersTypes = new List<Type>();

        public DynamicControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllerName = this.GetControllerName(request);
            var controllerMap = this.GetControllerMapping();
            if (controllerMap.ContainsKey(controllerName))
            {
                var controller = base.SelectController(request);
                return controller;
            }
            else
            {
                var matchedController =
                    _controllersTypes.FirstOrDefault(
                        i => i.Name.ToLower() == controllerName.ToLower() + "controller");
                var controller = new HttpControllerDescriptor(_configuration, controllerName, matchedController);
                return controller;
            }
        }

        public void AddController(ApiController controller)
        {
            _controllersTypes.Add(controller.GetType());
        }

        public void AddControllers(Assembly assembly)
        {
            var types = assembly.GetTypes();
            var matchedTypes = types.Where(t => typeof(ApiController).IsAssignableFrom(t)).ToList();
            foreach (var type in matchedTypes)
            {
                if (!_controllersTypes.Contains(type))
                {
                    _controllersTypes.Add(type);
                }
            }
        }
    }
}
