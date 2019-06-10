using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Rewrite;

namespace RewriteRules
{
    public static class RewriteOptionsExtensions
    {
        public static RewriteOptions AddRedirectToCanonicalUrl(this RewriteOptions options)
        {
            var rule = new RedirectToCanonicalUrlRule();
            options.Rules.Add(rule);
            return options;
        }

        public static RewriteOptions AddRedirectToCanonicalUrl(this RewriteOptions options, CanonicalUrlOptions ruleOptions)
        {
            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            options.Rules.Add(rule);
            return options;
        }
    }
}
