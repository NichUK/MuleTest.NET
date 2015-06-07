using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;

namespace MuleTest.NET
{
    public class MuleTestNet : IDisposable
    {
        private readonly IWindsorContainer _container;

        public MuleTestNet()
            : this(new WindsorContainer())
        {
        }

        public MuleTestNet(IWindsorContainer container)
        {
            _container = container;
        }

        public IWindsorContainer Container
        {
            get { return _container; }
        }

        public HttpServerMock.HttpServerMock CreateHttpServerMock(string url)
        {
            return new HttpServerMock.HttpServerMock(_container, url);
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
