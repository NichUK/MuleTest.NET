using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.Windsor;
using Moq;
using MuleTest.NET.HttpServerMock;

namespace MuleTest.NET.Tests
{
    public class UtilitiesController : ApiControllerBase<UtilitiesController>
    {
        private IWindsorContainer _container;

        public UtilitiesController(IWindsorContainer container, Mock<UtilitiesController> mock)
            : base(container, mock)
        {
        }

        [HttpPost]
        public virtual HttpResponseMessage Merge()
        {
            return this.Mock.Object.Merge();
        }
    }
}
