using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Moq;
using NUnit.Framework;

namespace MuleTest.NET.Tests
{
    [TestFixture]
    public class HttpServerMockTests
    {
        [Test]
        public void ShouldCallAMethodWithGet()
        {
            var muleTestNet = new MuleTestNet();
            using (var mockHttpServer = muleTestNet.CreateHttpServerMock("http://localhost:9091"))
            {
                var utilitiesMock = mockHttpServer.AddMock<UtilitiesController>("api/utilities/{action}", "merge");

                var request = "testing";
                var response = "testing";

                utilitiesMock.Setup(m => m.Merge()).Returns(() => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(response)
                });

                using (var content = new StringContent(request))
                {
                    using (var responseMessage = mockHttpServer.Http(HttpMethod.Post, "http://localhost:9091/api/utilities/merge", content))
                    {
                        var responseString = responseMessage.Content.ReadAsStringAsync().Result;
                        Assert.That(responseString, Is.EqualTo(response));
                    }
                }
            }
        }
    }
}
