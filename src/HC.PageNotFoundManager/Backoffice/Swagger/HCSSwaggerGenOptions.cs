using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
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

