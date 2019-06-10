# Rewrite Rules
[![Build Status](https://kyleherzog.visualstudio.com/ExtendableEnums/_apis/build/status/RewriteRules?branchName=develop)](https://kyleherzog.visualstudio.com/RewriteRules/_build/latest?definitionId=2?branchName=develop)

This library is available from [NuGet.org](https://www.nuget.org/packages/RewriteRules/).

--------------------------

A .NET Standard class library that provides predefined rewrite rules for ASP.net core. 

See the [changelog](CHANGELOG.md) for changes and roadmap.

## Rule Extension Methods

### AddRedirectToCanonicalUrl

This adds a rewrite rule that redirects requests to a standardly formated URL.  The is typically done for SEO purposes. 

```c#
var options = new RewriteOptions().AddRedirectToCanonicalUrl();
app.UseRewriter(options);
```

#### CanonicalUrlOptions
A `CanonicalUrlOptions` object can be passed to `AddRedirectToCanonicalUrl` to customize how the redirection is handled. The following are the properties that are available.

##### AlternativeHosts
This is a whitelist of host names other than the primary host name that will not be swapped for the primary host name.  This could be used in load-balanced environments to allow a host name for individual servers.

##### ExtensionsToInclude 
By default, URLs with a file name extension will not have the canonical URL rules applied. However, the file name extensions in this list will have the rules applied.  This list defaults to having common html file extensions in it (.html, .htm, .aspx, etc.).

##### IsForcingLowercase
A flag to indicate whether or not the URL should be lowercased.  This defaults to true.

##### PrimaryHost
The host name to redirect to if the DNS host name does not match this and is not listed in `AlternativeHosts`.

##### StatusCode
The status code to return if a redirect is determined to be needed.  This defaults to 301.

##### TrailingSlash
An enumeration value that determines what should be done with a trailing slash at the end of a URL.

## License
[MIT](LICENSE)