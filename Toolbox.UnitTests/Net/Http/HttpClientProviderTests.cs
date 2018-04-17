using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading;
using NUnit.Framework;
using Toolbox.Net.Http;

namespace Toolbox.UnitTests.Net.Http
{
    [TestFixture]
    public class HttpClientProviderTests
    {
        [TestFixture]
        public class GetHttpClient
        {
            [Test]
            public void ReturnsInstanceWithSpecifiedBaseUri()
            {
                var provider = new HttpClientProvider();
                var client = provider.GetHttpClient("http://alpha.com");

                Assert.That(client, Is.Not.Null);
                Assert.That(client.BaseAddress.Equals(new Uri("http://alpha.com")));

                var uri = new Uri("http://bravo.net");
                Assert.That(provider.GetHttpClient(uri).BaseAddress, Is.SameAs(uri));
            }

            [Test]
            public void ReturnsSameInstanceForSameUri()
            {
                var provider = new HttpClientProvider();
                var client1 = provider.GetHttpClient("http://alpha.com");
                var client2 = provider.GetHttpClient(new Uri("http://ALPHA.com"));

                Assert.That(client1, Is.SameAs(client2));
            }

            [Test]
            public void ReturnsSameInstanceIgnoringUriPath()
            {
                var provider = new HttpClientProvider();
                var client1 = provider.GetHttpClient("http://alpha.com/bravo.cshtml");
                var client2 = provider.GetHttpClient(new Uri("http://alpha.com/charlie.html"));

                Assert.That(client1, Is.SameAs(client2));
            }

            [Test]
            public void ReturnsSameInstanceIgnoringUriQueryString()
            {
                var provider = new HttpClientProvider();
                var client1 = provider.GetHttpClient("http://alpha.com/bravo?c=charlie");
                var client2 = provider.GetHttpClient(new Uri("http://alpha.com/bravo/?c=delta"));

                Assert.That(client1, Is.SameAs(client2));
            }

            [Test]
            public void ThrowsIfUriIsRelative()
            {
                var provider = new HttpClientProvider();
                try
                {
                    provider.GetHttpClient(new Uri("/bravo?c=charlie", UriKind.Relative));
                }
                catch (ArgumentException ex)
                {
                    Assert.That(ex.Message, Does.Contain("must be absolute"));
                    return;
                }

                Assert.Fail("The expected exception was not thrown.");
            }

            [Test]
            public void ReturnsNewInstanceForDifferentUri()
            {
                var provider = new HttpClientProvider();
                var client1 = provider.GetHttpClient("http://alpha.com");
                var client2 = provider.GetHttpClient(new Uri("http://bravo.net"));

                Assert.That(client1, Is.Not.SameAs(client2));
            }

            [Test]
            public void ReturnsNewInstanceForDifferentScheme()
            {
                var provider = new HttpClientProvider();
                var client1 = provider.GetHttpClient("http://alpha.com");
                var client2 = provider.GetHttpClient("https://alpha.com");

                Assert.That(client1, Is.Not.SameAs(client2));
            }

            [Test]
            public void ReturnsNewInstanceForDifferentPort()
            {
                var provider = new HttpClientProvider();
                var client1 = provider.GetHttpClient("http://alpha.com");
                var client2 = provider.GetHttpClient("http://alpha.com:8080");

                Assert.That(client1, Is.Not.SameAs(client2));
            }

            [Test]
            public void CallsInitializerForNewInstance()
            {
                var provider = new HttpClientProvider();
                var calls = new List<HttpClient>();
                var client = provider.GetHttpClient("http://alpha", h => calls.Add(h));

                Assert.That(calls.Count, Is.EqualTo(1));
                Assert.That(calls[0], Is.SameAs(client));
            }

            [Test]
            public void ReturnsNewInstanceIfTimeToLiveHasExpired()
            {
                var provider = new HttpClientProvider {
                    TimeToLive = new TimeSpan(0, 0, 0, 0, 100)
                };
                var client1 = provider.GetHttpClient("http://alpha");
                Thread.Sleep(150);
                var client2 = provider.GetHttpClient("http://alpha");

                Assert.That(client1, Is.Not.SameAs(client2));
            }
        }

        [TestFixture]
        public class Invalidate
        {
            [Test]
            public void RemovesSpecifiedUriFromCache()
            {
                var provider = new HttpClientProvider();
                var client1 = provider.GetHttpClient("http://alpha");
                provider.Invalidate(new Uri("http://alpha"));
                var client2 = provider.GetHttpClient("http://alpha");

                Assert.That(client1, Is.Not.SameAs(client2));
            }
        }
    }
}