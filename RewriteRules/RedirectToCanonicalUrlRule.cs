using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RewriteRules
{
    public class RedirectToCanonicalUrlRule : IRule
    {
        public RedirectToCanonicalUrlRule(CanonicalUrlOptions options)
        {
            Options = options;
        }

        public CanonicalUrlOptions Options { get; }

        public void ApplyRule(RewriteContext context)
        {
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
            if (Options.PrimaryHost.HasValue && host != Options.PrimaryHost && !Options.AlternateHosts.Contains(host))
            {
                host = Options.PrimaryHost;
            }

            var newUrl = UriHelper.BuildAbsolute(request.Scheme, host, request.PathBase, path, request.QueryString);

            if (Options.IsForcingLowercase)
            {
                newUrl = newUrl.ToLower(CultureInfo.CurrentCulture);
            }

            if (originalUrl != newUrl)
            {
                var response = context.HttpContext.Response;
                response.StatusCode = Options.StatusCode;
                response.Headers[HeaderNames.Location] = newUrl;
                context.Result = RuleResult.EndResponse;
                if (context.Logger != null)
                {
                    var message = LoggerMessage.Define(LogLevel.Information, 114, "Redirected to canonical URL");
                    message(context.Logger, null);
                }
            }
            else
            {
                context.Result = RuleResult.ContinueRules;
            }
        }
    }
}
