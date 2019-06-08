using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace RewriteRules.UnitTests.RedirectToCanonicalUrlRuleTests
{
    [TestClass]
    public class ApplyRuleShould
    {
        [TestMethod]
        public async Task AddsTrailingSlashGivenTrailingSlashSetToAdd()
        {
            var ruleOptions = new CanonicalUrlOptions
            {
                TrailingSlash = TrailingSlashAction.Add
            };
            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("foobar").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foobar/", response.Headers.Location.OriginalString);
            }
        }

        [TestMethod]
        public async Task MakeNoModificationGivenNoRedirectNeeded()
        {
            var ruleOptions = new CanonicalUrlOptions
            {
            };
            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://something.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("foo").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakeNoModificationGivenPathWithExtensionAndNotIncluded()
        {
            var ruleOptions = new CanonicalUrlOptions
            {
                IsForcingLowercase = true
            };

            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://something.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("FOo.jpg").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakesNoModificationGivenAddTrailingSlashSetButHasFileExtension()
        {
            var ruleOptions = new CanonicalUrlOptions()
            {
                TrailingSlash = TrailingSlashAction.Add
            };

            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("foobar.txt").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakesNoModificationGivenAddTrailingSlashSetButPresent()
        {
            var ruleOptions = new CanonicalUrlOptions()
            {
                TrailingSlash = TrailingSlashAction.Add
            };

            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("foobar/").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task MakesNoModificationGivenForceRemoveTrailingSlashSetButNotPresent()
        {
            var ruleOptions = new CanonicalUrlOptions()
            {
                TrailingSlash = TrailingSlashAction.Remove
            };

            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("foobar").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status404NotFound, (int)response.StatusCode);
            }
        }

        [TestMethod]
        public async Task RedirectGivenExtensionPresentAndIncluded()
        {
            var ruleOptions = new CanonicalUrlOptions
            {
                PrimaryHost = new HostString("example.com")
            };
            ruleOptions.ExtensionsToInclude.Add(".jpg");

            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://something.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("foo.JPG").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foo.jpg", response.Headers.Location.OriginalString);
            }
        }

        [TestMethod]
        public async Task RedirectGivenPrimaryHostNameSpecifiedButNotMatching()
        {
            var ruleOptions = new CanonicalUrlOptions
            {
                PrimaryHost = new HostString("example.com")
            };
            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://something.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("foo").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foo", response.Headers.Location.OriginalString);
            }
        }

        [TestMethod]
        public async Task RedirectGivenUppercaseInUrlAndReqiringLowercase()
        {
            var ruleOptions = new CanonicalUrlOptions();
            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("fooBar").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foobar", response.Headers.Location.OriginalString);
            }
        }

        [TestMethod]
        public async Task RemovesTrailingSlashGivenForceRemoveTrailingSlashSet()
        {
            var ruleOptions = new CanonicalUrlOptions
            {
                TrailingSlash = TrailingSlashAction.Remove
            };
            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            var options = new RewriteOptions();
            options.Rules.Add(rule);
            var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseRewriter(options);
            });
            using (var server = new TestServer(builder))
            {
                server.BaseAddress = new Uri("http://example.com");
                var client = server.CreateClient();
                var response = await client.GetAsync("foobar/").ConfigureAwait(true);

                Assert.AreEqual(StatusCodes.Status301MovedPermanently, (int)response.StatusCode);
                Assert.AreEqual("http://example.com/foobar", response.Headers.Location.OriginalString);
            }
        }
    }
}