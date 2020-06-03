using System;
using Microsoft.AspNetCore.Rewrite;

namespace RewriteRules
{
    /// <summary>
    /// Provides extension methods for <see cref="RewriteOptions"/> objects.
    /// </summary>
    public static class RewriteOptionsExtensions
    {
        /// <summary>
        /// Adds a rule to redirect to a canonical URL.
        /// </summary>
        /// <param name="options">The options that define how the canonical URL redirection will take place.</param>
        /// <returns>The original <see cref="RewriteOptions"/> passed in.</returns>
        public static RewriteOptions AddRedirectToCanonicalUrl(this RewriteOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var rule = new RedirectToCanonicalUrlRule();
            options.Rules.Add(rule);
            return options;
        }

        /// <summary>
        /// Adds a rule to redirect to a canonical URL.
        /// </summary>
        /// <param name="options">The <see cref="RewriteOptions"/> onto which the rules will be applied.</param>
        /// <param name="ruleOptions">The options that define how the canonical URL redirection will take place.</param>
        /// <returns>The original <see cref="RewriteOptions"/> passed in.</returns>
        public static RewriteOptions AddRedirectToCanonicalUrl(this RewriteOptions options, CanonicalUrlOptions ruleOptions)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var rule = new RedirectToCanonicalUrlRule(ruleOptions);
            options.Rules.Add(rule);
            return options;
        }
    }
}
