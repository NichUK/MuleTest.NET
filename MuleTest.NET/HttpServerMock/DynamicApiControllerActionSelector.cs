using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace MuleTest.NET.HttpServerMock
{
    public class DynamicApiControllerActionSelector : ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            IHttpRouteData routeData = controllerContext.RouteData;
            bool containsAction = routeData.Values.ContainsKey("action");

            if (containsAction)
            {
                var action = base.SelectAction(controllerContext);
                return action;
            }

            try
            {
                var result = base.SelectAction(controllerContext);
                return result;
            }
            finally
            {
                routeData.Values.Remove("action");
            }
        }
    }
}
