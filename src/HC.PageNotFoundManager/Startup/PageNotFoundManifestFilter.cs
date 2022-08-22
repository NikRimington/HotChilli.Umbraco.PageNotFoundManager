

using System.Collections.Generic;
using System.Diagnostics;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.Semver;
using Umbraco.Extensions;

namespace HC.PageNotFoundManager.Startup
{
    internal class PageNotFoundManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(
                new PackageManifest
                {
                    PackageName = "HC.PageNotFoundManager",
                    Scripts = new[]
                              {
                                  "/App_Plugins/HC.PageNotFound/js/resource.js",
                                  "/App_Plugins/HC.PageNotFound/js/dialog.controller.js"
                              },
                    Version = GetVersion()
                });
        }

        private static string GetVersion()
        {
            var assembly = typeof(PageNotFoundManifestFilter).Assembly;
            try
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.GetAssemblyFile().FullName);
                if (fileVersionInfo.ProductVersion != null && SemVersion.TryParse(
                        fileVersionInfo.ProductVersion,
                        out var productVersion))
                {
                    return productVersion.ToSemanticStringWithoutBuild();
                }
            }
            catch
            {
                //default to assembly version
            }

            return assembly.GetName().Version.ToString(3);
        }
    }
}