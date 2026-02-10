using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Hmmh.Api.OpenApi;

/// <summary>
///     Adds an OAuth token request body definition for the /connect/token endpoint.
/// </summary>
public sealed class TokenEndpointOperationFilter : IOperationFilter
{
    /// <summary>
    ///     Applies the form-url-encoded request schema to the token endpoint.
    /// </summary>
    /// <param name="operation">Swagger operation being generated.</param>
    /// <param name="context">Context for the operation generation.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Restrict customization to the token endpoint only.
        var relativePath = "/" + context.ApiDescription.RelativePath?.TrimStart('/');
        if (!string.Equals(relativePath, "/connect/token", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        operation.Summary = "Exchange credentials or refresh tokens for access tokens.";
        operation.Description = "Use application/x-www-form-urlencoded with grant_type=password or grant_type=refresh_token.";

        operation.RequestBody = new OpenApiRequestBody
        {
            Required = true,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/x-www-form-urlencoded"] = new()
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["grant_type"] = new()
                            {
                                Type = "string",
                                Description = "password or refresh_token",
                                Default = new OpenApiString("password"),
                            },
                            ["username"] = new()
                            {
                                Type = "string",
                                Description = "Required for password grant",
                            },
                            ["password"] = new()
                            {
                                Type = "string",
                                Description = "Required for password grant",
                                Format = "password",
                            },
                            ["refresh_token"] = new()
                            {
                                Type = "string",
                                Description = "Required for refresh_token grant",
                            },
                            ["scope"] = new()
                            {
                                Type = "string",
                                Description = "Space-delimited scopes (e.g. openid api offline_access)",
                                Default = new OpenApiString("openid api offline_access"),
                            },
                            ["client_id"] = new()
                            {
                                Type = "string",
                                Description = "Client id (use hmmh-ui)",
                                Default = new OpenApiString("hmmh-ui"),
                            },
                        },
                        Required = new HashSet<string> { "grant_type" },
                    },
                },
            },
        };
    }
}
