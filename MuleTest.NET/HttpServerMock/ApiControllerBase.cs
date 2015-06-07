using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moq;

namespace MuleTest.NET.HttpServerMock
{
    public class ApiControllerBase<T> : ApiController where T:class 
    {
        private readonly Mock<T> _mock;

        private readonly IWindsorContainer _container;

        public ApiControllerBase(IWindsorContainer container)
            : this(container, new Mock<T>())
        {
        }

        public ApiControllerBase(IWindsorContainer container, Mock<T> mock)
        {
            _mock = mock;
            _container = container;
        }

        public Mock<T> Mock
        {
            get { return this._mock; }
        }
    }
}
