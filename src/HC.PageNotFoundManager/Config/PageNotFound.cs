namespace HC.PageNotFoundManager.Config;

/// <summary>
///     Class to represent the settings from appsettings
/// </summary>
public class PageNotFound
{
    public PageNotFound() => UserGroups = [];

    public string[] UserGroups { get; set; }
}