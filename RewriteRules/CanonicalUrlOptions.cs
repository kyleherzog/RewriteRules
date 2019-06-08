using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RewriteRules
{
    public class CanonicalUrlOptions
    {
        public int StatusCode { get; set; } = StatusCodes.Status301MovedPermanently;

        public TrailingSlashAction TrailingSlash { get; set; }

        public bool IsForcingLowercase { get; set; } = true;

        public HostString PrimaryHost { get; set; }

        public IList<HostString> AlternateHosts { get; } = new List<HostString>();

        public IList<string> ExtensionsToInclude { get; } = new List<string> { ".html", ".htm", ".aspx", ".asp"};
    }
}
