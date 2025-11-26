using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
#if NET10_0
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
#endif
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HC.PageNotFoundManager.Backoffice.Swagger;

public class HCSSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        options.SwaggerDoc("hcs", new OpenApiInfo { Title = "Hot Chilli Api", Version = "1.0" });
        options.OperationFilter<HCSBackofficeFilter>();
    }
}

