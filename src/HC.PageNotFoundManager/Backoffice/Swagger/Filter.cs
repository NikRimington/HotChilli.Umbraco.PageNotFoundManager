using Umbraco.Cms.Api.Management.OpenApi;

namespace HC.PageNotFoundManager.Backoffice.Swagger;

public class HCSBackofficeFilter : BackOfficeSecurityRequirementsOperationFilterBase
{
    protected override string ApiName => "hcs";
}

