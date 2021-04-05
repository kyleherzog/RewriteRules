using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace RewriteRules
{
    /// <summary>
    /// A rewrite rule to redirect to a canonical URL.
    /// </summary>
    public class RedirectToCanonicalUrlRule : IRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectToCanonicalUrlRule"/> class.
        /// </summary>
        public RedirectToCanonicalUrlRule()
            : this(new CanonicalUrlOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectToCanonicalUrlRule"/> class.
        /// </summary>
        /// <param name="options">The <see cref="CanonicalUrlOptions" /> to be applied.</param>
        public RedirectToCanonicalUrlRule(CanonicalUrlOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// Gets the <see cref="CanonicalUrlOptions"/> that are to be used.
        /// </summary>
        public CanonicalUrlOptions Options { get; }

        /// <inheritdoc/>
        public void ApplyRule(RewriteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var request = context.HttpContext.Request;

            var originalUrl = request.GetDisplayUrl();

            var path = request.Path;

            var extension = Path.GetExtension(path);
            if (!string.IsNullOrEmpty(extension) && !Options.ExtensionsToInclude.Any(x => x.Equals(extension, StringComparison.OrdinalIgnoreCase)))
            {
                context.Result = RuleResult.ContinueRules;
                return;
            }

            if (Options.TrailingSlash != TrailingSlashAction.Ignore && path.HasValue)
            {
                if (Options.TrailingSlash == TrailingSlashAction.Remove)
                {
                    path = new PathString(path.Value.TrimEnd('/'));
                }
                else if (Options.TrailingSlash == TrailingSlashAction.Add && !path.Value.Contains(".") && path.Value[path.Value.Length - 1] != '/')
                {
                    path = new PathString($"{path.Value}/");
                }
            }

            var host = request.Host;
            if (Options.PrimaryHost.HasValue && host != Options.PrimaryHost && !host.Value.StartsWith("localhost", StringComparison.OrdinalIgnoreCase) && !Options.AlternateHosts.Contains(host))
            {
                host = Options.PrimaryHost;
            }

            var newUrl = UriHelper.BuildAbsolute(request.Scheme, host, request.PathBase, path, request.QueryString);

            if (Options.IsForcingLowercase)
            {
                if (Options.ShouldApplyToQuery)
                {
                    newUrl = newUrl.ToLower(CultureInfo.CurrentCulture);
                }
                else
                {
                    var loweredHost = new HostString(host.Host.ToLower(CultureInfo.CurrentCulture));
                    if (host.Port.HasValue)
                    {
                        loweredHost = new HostString(host.Host.ToLower(CultureInfo.CurrentCulture), host.Port.Value);
                    }

                    newUrl = UriHelper.BuildAbsolute(
                        request.Scheme.ToLower(CultureInfo.CurrentCulture),
                        loweredHost,
                        new PathString(request.PathBase.Value.ToLower(CultureInfo.CurrentCulture)),
                        new PathString(path.Value.ToLower(CultureInfo.CurrentCulture)),
                        request.QueryString);
                }
            }

            if (HttpUtility.UrlDecode(originalUrl) != HttpUtility.UrlDecode(newUrl))
            {
                var response = context.HttpContext.Response;
                response.StatusCode = Options.StatusCode;
                response.Headers[HeaderNames.Location] = newUrl;
                context.Result = RuleResult.EndResponse;
                context.Logger?.LogInformation("Redirected from {OriginalUrl} to canonical URL {NewUrl}", originalUrl, newUrl);
            }
            else
            {
                context.Result = RuleResult.ContinueRules;
            }
        }
    }
}
