using System.Linq;

namespace HC.PageNotFoundManager.Models
{
    public class PageNotFoundManagerSettings
    {
        public const string Key = "HCS:PageNotFoundManager";

        private static readonly string[] DefaultExcludePaths = ["/umbraco-signin"];

        private string[] _excludePaths = [];

        public string[] ExcludePaths
        {
            get => DefaultExcludePaths.Concat(_excludePaths ?? []).Distinct().ToArray();
            set => _excludePaths = value ?? [];
        }
    }
}
