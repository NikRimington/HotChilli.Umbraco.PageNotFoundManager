using System;

namespace HC.PageNotFoundManager.Core.Config
{
    /// <summary>
    /// Class to represent the settings from appsettings
    /// </summary>
    public class PageNotFound
    {
        public PageNotFound()
        {
            UserGroups = Array.Empty<string>();
        }
        public string[] UserGroups { get; set; }
    }
}
