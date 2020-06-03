using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RewriteRules
{
    public class CanonicalUrlOptions
    {
        /// <summary>
        /// Gets a white-list of host names other than the primary host name that will not be swapped for the primary host name.
        /// </summary>
        public IList<HostString> AlternateHosts { get; } = new List<HostString>();

        /// <summary>
        /// Gets a list of file name extensions to target for canonical URL redirects.
        /// </summary>
        public IList<string> ExtensionsToInclude { get; } = new List<string> { ".html", ".htm", ".aspx", ".asp" };

        /// <summary>
        /// Gets or sets a value indicating whether or not the canonical URL should be lowercased.
        /// </summary>
        public bool IsForcingLowercase { get; set; } = true;

        /// <summary>
        /// Gets or sets the primary DNS host name to redirect to if not matched with this value and not listed in AlernateHosts.
        /// </summary>
        public HostString PrimaryHost { get; set; }

        /// <summary>
        /// Gets or sets the status code to use for redirection if a redirect is required.
        /// </summary>
        public int StatusCode { get; set; } = StatusCodes.Status301MovedPermanently;

        /// <summary>
        /// Gets or sets a value indicating the action to take with trailing backslashes.
        /// </summary>
        public TrailingSlashAction TrailingSlash { get; set; } = TrailingSlashAction.Remove;
    }
}
