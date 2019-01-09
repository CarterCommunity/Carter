namespace Carter.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Carter.Request;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing.Template;
    using Microsoft.OpenApi;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Extensions;
    using Microsoft.OpenApi.Models;

    public static class CarterOpenApi
    {
        public static RequestDelegate BuildOpenApiResponse(CarterOptions options, Dictionary<(string verb, string path), RouteMetaData> metaDatas)
        {
            return context =>
            {
                var document = new OpenApiDocument
                {
                    Info = new OpenApiInfo
                    {
                        Version = "3.0.0",
                        Title = options.OpenApi.DocumentTitle
                    },
                    Servers = new List<OpenApiServer>(options.OpenApi.ServerUrls.Select(x => new OpenApiServer { Url = x })),
                    Paths = new OpenApiPaths()
                };

                foreach (var routeMetaData in metaDatas.GroupBy(pair => pair.Key.path))
                {
                    var pathItem = new OpenApiPathItem();

                    //Get /foo/{id:int}
                    var template = TemplateParser.Parse(routeMetaData.Key);

                    //Turn it into /foo/{id}
                    var templateName = string.Join("/",
                        template.Segments.Select(x => string.Join(string.Empty,
                            x.Parts.Select(z => !z.IsParameter ? z.Text : "{" + (z.IsCatchAll ? "*" : string.Empty) + z.Name + (z.IsOptional ? "?" : string.Empty) + "}"))));

                    var methodRoutes = routeMetaData.Select(x => x);

                    foreach (var methodRoute in methodRoutes)
                    {
                        var operation = new OpenApiOperation { Tags = new List<OpenApiTag> { new OpenApiTag { Name = methodRoute.Value.Tag } }, Description = methodRoute.Value.Description };

                        CreateOpenApiRequestBody(methodRoute, operation);

                        CreateOpenApiResponseBody(methodRoute, operation);

                        CreateOpenApiRouteConstraints(template, operation);

                        var verb = CreateOpenApiOperationVerb(methodRoute);

                        pathItem.Operations.Add(verb, operation);
                    }

                    document.Paths.Add("/" + templateName, pathItem);
                }

                context.Response.ContentType = "application/json; charset=utf-8";
                document.SerializeAsJson(context.Response.Body, OpenApiSpecVersion.OpenApi3_0);
                return Task.CompletedTask;
            };
        }

        private static OperationType CreateOpenApiOperationVerb(KeyValuePair<(string verb, string path), RouteMetaData> methodRoute)
        {
            switch (methodRoute.Key.verb)
            {
                case "GET":
                    return OperationType.Get;
                case "POST":
                    return OperationType.Post;
                case "PUT":
                    return OperationType.Put;
                case "DELETE":
                    return OperationType.Delete;
                case "HEAD":
                    return OperationType.Head;
                case "OPTIONS":
                    return OperationType.Options;
                case "PATCH":
                    return OperationType.Patch;
                default:
                    return OperationType.Get;
            }
        }

        private static void CreateOpenApiRouteConstraints(RouteTemplate template, OpenApiOperation operation)
        {
            if (template.Parameters.Any())
            {
                operation.Parameters = template.Parameters.Select(x => new OpenApiParameter
                {
                    Name = x.Name,
                    In = ParameterLocation.Path,
                    Required = true,
                    Schema = new OpenApiSchema { Type = GetOpenApiTypeMapping(x.InlineConstraints.FirstOrDefault()?.Constraint) }
                }).ToList();
            }
        }

        private static void CreateOpenApiResponseBody(KeyValuePair<(string verb, string path), RouteMetaData> methodRoute, OpenApiOperation operation)
        {
            if (methodRoute.Value.Responses != null)
            {
                foreach (var valueStatusCode in methodRoute.Value.Responses)
                {
                    OpenApiResponse openApiResponse;
                    if (valueStatusCode.Response == null)
                    {
                        openApiResponse = new OpenApiResponse { Description = valueStatusCode.Description };
                    }
                    else
                    {
                        bool arrayType = false;
                        Type responseType;

                        if (valueStatusCode.Response.IsArray() || valueStatusCode.Response.IsCollection() || valueStatusCode.Response.IsEnumerable())
                        {
                            arrayType = true;
                            responseType = valueStatusCode.Response.GetElementType();
                            if (responseType == null)
                            {
                                responseType = valueStatusCode.Response.GetGenericArguments().First();
                            }
                        }
                        else
                        {
                            responseType = valueStatusCode.Response;
                        }

                        var propNames = responseType.GetProperties().Select(x => (Name: x.Name.ToLower(), Type: x.PropertyType.Name.ToLower()));

                        var arrbj = new OpenApiArray();
                        var propObj = new OpenApiObject();

                        foreach (var propertyInfo in propNames)
                        {
                            propObj.Add(propertyInfo.Name, new OpenApiString("")); //TODO Could use Bogus to generate some data rather than empty string
                        }

                        var schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = propNames.ToDictionary(key => key.Name, value => new OpenApiSchema { Type = GetOpenApiTypeMapping(value.Type) }),
                            Example = propObj
                        };

                        var arrayschema = new OpenApiSchema { Type = "array" };

                        if (arrayType)
                        {
                            arrbj.Add(propObj);
                            arrayschema.Items = schema;
                        }

                        var respObj = arrayType ? new OpenApiObject { { responseType.Name.ToLower(), arrbj } } : propObj;

                        openApiResponse = new OpenApiResponse
                        {
                            Description = valueStatusCode.Description,
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                {
                                    "application/json",
                                    new OpenApiMediaType
                                    {
                                        //Example = respObj
                                        Schema = arrayType ? arrayschema : schema
                                    }
                                }
                            }
                        };
                    }

                    operation.Responses.Add(valueStatusCode.Code.ToString(), openApiResponse);
                }
            }
        }

        private static void CreateOpenApiRequestBody(KeyValuePair<(string verb, string path), RouteMetaData> keyValuePair, OpenApiOperation operation)
        {
            if (keyValuePair.Value.Request != null)
            {
                bool arrayType = false;
                Type requestType;

                if (keyValuePair.Value.Request.IsArray() || keyValuePair.Value.Request.IsCollection() || keyValuePair.Value.Request.IsEnumerable())
                {
                    arrayType = true;
                    requestType = keyValuePair.Value.Request.GetElementType();
                    if (requestType == null)
                    {
                        requestType = keyValuePair.Value.Request.GetGenericArguments().FirstOrDefault();
                    }
                }
                else
                {
                    requestType = keyValuePair.Value.Request;
                }

                var propNames = requestType.GetProperties().Select(x => (Name: x.Name.ToLower(), Type: x.PropertyType.Name.ToLower()));

                var arrbj = new OpenApiArray();
                var propObj = new OpenApiObject();

                foreach (var propertyInfo in propNames)
                {
                    propObj.Add(propertyInfo.Name, new OpenApiString("")); //TODO Could use Bogus to generate some data rather than empty string
                }

                var schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = propNames.ToDictionary(key => key.Name, value => new OpenApiSchema { Type = GetOpenApiTypeMapping(value.Type) }),
                    Example = propObj
                };

                var arrayschema = new OpenApiSchema { Type = "array" };

                if (arrayType)
                {
                    arrbj.Add(propObj);
                    arrayschema.Items = schema;
                }

                //var reqObj = arrayType ? new OpenApiObject { { requestType.Name.ToLower(), arrbj } } : propObj;

                var requestBody = new OpenApiRequestBody();
                requestBody.Content.Add("application/json",
                    new OpenApiMediaType
                    {
                        //Example = reqObj,
                        Schema = arrayType ? arrayschema : schema
                    });

                operation.RequestBody = requestBody;
            }
        }

        private static string GetOpenApiTypeMapping(string constraint)
        {
            if (constraint == null)
            {
                return string.Empty;
            }

            switch (constraint)
            {
                case "int32":
                case "long64":
                case "int":
                case "long":
                    return "integer";

                case "single":
                case "decimal":
                case "double":
                case "float":
                    return "number";

                case "boolean":
                case "bool":
                    return "boolean";

                case "datetime":
                    return "string";

                default:
                    return "string";
            }
        }
    }
}
