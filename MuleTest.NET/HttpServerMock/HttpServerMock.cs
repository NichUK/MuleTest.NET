using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.Owin.Hosting;
using Moq;
using Owin;

namespace MuleTest.NET.HttpServerMock
{
    public class HttpServerMock : IDisposable
    {
        private IDisposable _webHost;

        private string _url;

        private IWindsorContainer _container;

        private Action<IAppBuilder> _startAction;

        private static HttpConfiguration _httpConfiguration = new HttpConfiguration(); 

        internal HttpServerMock(IWindsorContainer container, string url)
            : this(container, url, appBuilder =>
            {
                // Configure Web API for self-host. 
                _httpConfiguration.Services.Replace(typeof(IHttpControllerSelector), container.Resolve<DynamicControllerSelector>());
                _httpConfiguration.Services.Replace(typeof(IHttpControllerActivator), container.Resolve<CastleControllerActivator>());
                _httpConfiguration.Services.Replace(typeof(IHttpActionSelector), container.Resolve<DynamicApiControllerActionSelector>());
                appBuilder.UseWebApi(_httpConfiguration);
            })
        {
        }
        
        internal HttpServerMock(IWindsorContainer container, string url, Action<IAppBuilder> startAction)
        {
            if (startAction == null)
            {
                throw new ArgumentNullException("startAction", "You must pass a Start Action");
            }
            _container = container;
            _startAction = startAction;
            _url = url;
            _container.Register(Component.For<IWindsorContainer>().Instance(_container).LifestyleSingleton());
            _container.Register(Component.For<HttpConfiguration>().Instance(_httpConfiguration).LifestyleSingleton());
            _container.Install(FromAssembly.InThisApplication());
            _container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(Assembly.GetExecutingAssembly().Location)));
            this.StartUp();
        }

        public void StartUp()
        {
            _webHost = WebApp.Start(_url, _startAction);
        }

        public void ShutDown()
        {
            this.Dispose();
        }

        public Mock<T> AddMock<T>(string route, string action) where T : ApiControllerBase<T>
        {
            var controllerType = typeof (T);
            var controllerLongName = controllerType.Name;
            var controllerName = controllerLongName.Substring(0, controllerLongName.Length - "Controller".Length);

            _httpConfiguration.Routes.MapHttpRoute(
                name: controllerLongName + "Route",
                routeTemplate: route,
                defaults: new { controller = controllerName, action = action }
            );

            _container.Register(Component.For<T>().ImplementedBy<T>().LifestyleTransient());
            var typeMock = new Mock<T>(_container, null);
            _container.Register(Component.For<Mock<T>>().Instance(typeMock));
            return typeMock;
        }

        public HttpResponseMessage Http(HttpMethod httpMethod, string url, HttpContent payload)
        {
            using (var webRequestHandler = new WebRequestHandler())
            {
                // if we need to deal with self-signed certs, do it like this
                ////X509Certificate2 certificate = GetMyX509Certificate();
                ////webRequestHandler.ClientCertificates.Add(certificate);

                // if we need to deal with network credentials do it like this
                ////var credentials = new NetworkCredential("username", "password");
                ////webRequestHandler.Credentials = credentials;
                
                using (var client = new HttpClient(webRequestHandler))
                {
                    switch (httpMethod.ToString())
                    {
                        case "GET":
                            var getResponseMessage = client.GetAsync(url).Result;
                            return getResponseMessage;
                            break;

                        case "POST":
                            var postResponseMessage = client.PostAsync(url, payload).Result;
                            return postResponseMessage;
                            break;

                        case "PUT":
                            var putResponseMessage = client.PutAsync(url, payload).Result;
                            return putResponseMessage;
                            break;

                        case "DELETE":
                            var deleteResponseMessage = client.DeleteAsync(url).Result;
                            return deleteResponseMessage;
                            break;

                        default:
                            break;
                    }
                }

                return null;
            }
        }

        public void Dispose()
        {
            if (_webHost != null)
            {
                _webHost.Dispose();
            }
        }
    }
}
