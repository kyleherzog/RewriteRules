using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RewriteRules
{
    public class CanonicalUrlOptions
    {
        public IList<HostString> AlternateHosts { get; } = new List<HostString>();

        public IList<string> ExtensionsToInclude { get; } = new List<string> { ".html", ".htm", ".aspx", ".asp" };

        public bool IsForcingLowercase { get; set; } = true;

        public HostString PrimaryHost { get; set; }

        public int StatusCode { get; set; } = StatusCodes.Status301MovedPermanently;

        public TrailingSlashAction TrailingSlash { get; set; }
    }
}
