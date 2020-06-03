namespace RewriteRules
{
    /// <summary>
    /// An list of actions that can take place when checking for the trailing slash on a URL.
    /// </summary>
    public enum TrailingSlashAction
    {
        /// <summary>
        /// Ignore whether or not there is a trailing slash.
        /// </summary>
       Ignore,

       /// <summary>
       /// Remove the trailing slash if it exists.
       /// </summary>
       Remove,

       /// <summary>
       /// Add the trailing slash if it is missing.
       /// </summary>
       Add,
    }
}
