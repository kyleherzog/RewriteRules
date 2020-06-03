using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RewriteRules.UnitTests.RedirectToCanonicalUrlRuleTests
{
    [TestClass]
    public class ApplyRuleShould
    {
        [TestMethod]
        public async Task NotRedirectGivenEscapedCharacter()
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    var options = new RewriteOptions().AddRedirectToCanonicalUrl();
                    app.UseRewriter(options);
                });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://something.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foo%3fp=1")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task AddTrailingSlashGivenTrailingSlashSetToAdd()
        {
            var canonicalOptions = new CanonicalUrlOptions
            {
                TrailingSlash = TrailingSlashAction.Add
            };

            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foobar")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foobar/", response.Headers.Location.OriginalString);
            }
        }

        [TestMethod]
        public async Task MakeNoModificationGivenAddTrailingSlashSetButHasFileExtension()
        {
            var canonicalOptions = new CanonicalUrlOptions()
            {
                TrailingSlash = TrailingSlashAction.Add
            };

            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);

            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foobar.txt")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakeNoModificationGivenForceRemoveTrailingSlashSetButNotPresent()
        {
            var canonicalOptions = new CanonicalUrlOptions()
            {
                TrailingSlash = TrailingSlashAction.Remove
            };

            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foobar")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakeNoModificationGivenNoRedirectNeeded()
        {
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                var options = new RewriteOptions().AddRedirectToCanonicalUrl();
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://something.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foo")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakeNoModificationGivenPathWithExtensionAndNotIncluded()
        {
            var canonicalOptions = new CanonicalUrlOptions
            {
                IsForcingLowercase = true
            };

            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://something.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}FOo.jpg")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakeNoModifictionGivenPrimaryHostNameSpecifiedNotMatchingButIsAlternate()
        {
            var canonicalOptions = new CanonicalUrlOptions
            {
                PrimaryHost = new HostString("example.com")
            };
            canonicalOptions.AlternateHosts.Add(new HostString("dev.example.com"));
            canonicalOptions.AlternateHosts.Add(new HostString("test.example.com"));
            canonicalOptions.AlternateHosts.Add(new HostString("stage.example.com"));

            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://test.example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foo")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakeNoModifictionGivenUrlIsLocalHost()
        {
            var canonicalOptions = new CanonicalUrlOptions
            {
                PrimaryHost = new HostString("example.com")
            };

            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://localhost:12345");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foo")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakesNoModificationGivenAddTrailingSlashSetButPresent()
        {
            var canonicalOptions = new CanonicalUrlOptions()
            {
                TrailingSlash = TrailingSlashAction.Add
            };

            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);

            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foobar/")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task RedirectGivenExtensionPresentAndIncluded()
        {
            var canonicalOptions = new CanonicalUrlOptions
            {
                PrimaryHost = new HostString("example.com")
            };
            canonicalOptions.ExtensionsToInclude.Add(".jpg");

            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);

            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://something.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foo.JPG")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foo.jpg", response.Headers.Location.OriginalString);
            }
        }

        [TestMethod]
        public async Task RedirectGivenPrimaryHostNameSpecifiedButNotMatching()
        {
            var canonicalOptions = new CanonicalUrlOptions
            {
                PrimaryHost = new HostString("example.com")
            };
            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);

            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://something.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foo")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foo", response.Headers.Location.OriginalString);
            }
        }

        [TestMethod]
        public async Task RedirectGivenUppercaseInUrlAndReqiringLowercase()
        {
            var options = new RewriteOptions().AddRedirectToCanonicalUrl();

            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}fooBar")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foobar", response.Headers.Location.OriginalString);
            }
        }

        [TestMethod]
        public async Task RedirectWithCustomResponseCodeGivenCustomResponseCodeSpecified()
        {
            var canonicalOptions = new CanonicalUrlOptions
            {
                StatusCode = StatusCodes.Status302Found
            };

            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);

            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}fooBar")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status302Found, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foobar", response.Headers.Location.OriginalString);
            }
        }

        [TestMethod]
        public async Task RemoveTrailingSlashGivenForceRemoveTrailingSlashSet()
        {
            var canonicalOptions = new CanonicalUrlOptions
            {
                TrailingSlash = TrailingSlashAction.Remove
            };
            var options = new RewriteOptions().AddRedirectToCanonicalUrl(canonicalOptions);

            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync(new Uri($"{server.BaseAddress}foobar/")).ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foobar", response.Headers.Location.OriginalString);
            }
        }
    }
}
