namespace Carter.OpenApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Carter.Request;
    using FluentValidation.Validators;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing.Template;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi;
    using Microsoft.OpenApi.Any;
    using Microsoft.OpenApi.Extensions;
    using Microsoft.OpenApi.Models;

    internal static class CarterOpenApi
    {
        internal static RequestDelegate BuildOpenApiResponse(CarterOptions options, Dictionary<(string verb, string path), RouteMetaData> metaDatas)
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
                    Paths = new OpenApiPaths(),
                    Components = new OpenApiComponents()
                };
                foreach (var globalSecurity in options.OpenApi.GlobalSecurityDefinitions)
                {
                    var req = new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                                { Reference = new OpenApiReference { Id = globalSecurity, Type = ReferenceType.SecurityScheme }, UnresolvedReference = true },
                            new List<string>()
                        }
                    };
                    document.SecurityRequirements.Add(req);
                }

                foreach (var apiSecurity in options.OpenApi.Securities)
                {
                    var scheme = new OpenApiSecurityScheme
                    {
                        Name = apiSecurity.Value.Name,
                        Type = (SecuritySchemeType)Enum.Parse(typeof(SecuritySchemeType), apiSecurity.Value.Type.ToString(), ignoreCase: true),
                        Scheme = apiSecurity.Value.Scheme,
                        BearerFormat = apiSecurity.Value.BearerFormat,
                        In = (ParameterLocation)Enum.Parse(typeof(ParameterLocation), apiSecurity.Value.In.ToString(), ignoreCase: true)
                    };

                    document.Components.SecuritySchemes.Add(apiSecurity.Key, scheme);
                }

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
                        var operation = new OpenApiOperation
                        {
                            Description = methodRoute.Value.Description,
                            OperationId = methodRoute.Value.OperationId
                        };

                        if (!string.IsNullOrWhiteSpace(methodRoute.Value.Tag))
                        {
                            operation.Tags = new List<OpenApiTag> { new OpenApiTag { Name = methodRoute.Value.Tag } };
                        }

                        if (!string.IsNullOrWhiteSpace(methodRoute.Value.SecuritySchema))
                        {
                            operation.Security = new List<OpenApiSecurityRequirement>(new[]
                            {
                                new OpenApiSecurityRequirement
                                {
                                    {
                                        new OpenApiSecurityScheme
                                            { Reference = new OpenApiReference { Id = methodRoute.Value.SecuritySchema, Type = ReferenceType.SecurityScheme }, UnresolvedReference = true },
                                        new List<string>()
                                    }
                                }
                            });
                        }

                        CreateOpenApiRequestBody(document, methodRoute, operation, context);

                        CreateOpenApiResponseBody(document, methodRoute, operation);

                        CreateOpenApiRouteConstraints(template, operation);

                        CreateOpenApiQueryStringParameters(operation, methodRoute.Value.QueryStringParameter);

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

        private static void CreateOpenApiQueryStringParameters(OpenApiOperation operation, QueryStringParameter[] queryStringParameters)
        {
            if (queryStringParameters == null || !queryStringParameters.Any())
            {
                return;
            }

            foreach (var queryStringParameter in queryStringParameters)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    In = ParameterLocation.Query,
                    Required = queryStringParameter.Required,
                    Name = queryStringParameter.Name,
                    Description = queryStringParameter.Description,
                    Schema = new OpenApiSchema { Type = GetOpenApiTypeMapping(queryStringParameter.Type.Name.ToLower()) }
                });
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

        private static void CreateOpenApiResponseBody(OpenApiDocument document, KeyValuePair<(string verb, string path), RouteMetaData> methodRoute, OpenApiOperation operation)
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
                        var responseTypeName = string.Empty;
                        var singularTypeName = string.Empty;

                        if (valueStatusCode.Response.IsArray() || valueStatusCode.Response.IsCollection() || valueStatusCode.Response.IsEnumerable())
                        {
                            arrayType = true;
                            responseType = valueStatusCode.Response.GetElementType();
                            if (responseType == null)
                            {
                                responseType = valueStatusCode.Response.GetGenericArguments().First();
                                responseTypeName = responseType.Name + "s";
                                singularTypeName = responseType.Name;
                            }
                        }
                        else
                        {
                            responseType = valueStatusCode.Response;
                            responseTypeName = responseType.Name;
                        }

                        var propNames = responseType.GetProperties()
                            .Select(x => (Name: x.Name.ToLower(), Type: x.PropertyType.Name.ToLower()))
                            .ToList();

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
                                        Schema = new OpenApiSchema { Reference = new OpenApiReference { Id = responseTypeName, Type = ReferenceType.Schema } }
                                        //Schema = arrayType ? arrayschema : schema
                                    }
                                }
                            }
                        };

                        if (!document.Components.Schemas.ContainsKey(responseTypeName))
                        {
                            if (!arrayType)
                            {
                                document.Components.Schemas.Add(responseTypeName, schema);
                            }
                            else
                            {
                                document.Components.Schemas.Add(responseTypeName,
                                    new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Reference = new OpenApiReference { Id = singularTypeName, Type = ReferenceType.Schema } } });
                                //TODO Should we check that at the end that any components that are "array" types have a component registered of the  singularTypeName for example you could have IEnumerable<Foo> but Foo is not used in another route so won't be registered in components
                            }
                        }
                    }

                    operation.Responses.Add(valueStatusCode.Code.ToString(), openApiResponse);
                }
            }
        }

        private static void CreateOpenApiRequestBody(OpenApiDocument document, KeyValuePair<(string verb, string path), RouteMetaData> keyValuePair, OpenApiOperation operation, HttpContext context)
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
                        requestType = keyValuePair.Value.Request.GetGenericArguments().First();
                    }
                }
                else
                {
                    requestType = keyValuePair.Value.Request;
                }

                var propNames = requestType.GetProperties()
                    .Select(x => (Name: x.Name.ToLower(), Type: x.PropertyType.Name.ToLower()))
                    .ToList();

                var arrbj = new OpenApiArray();
                var propObj = new OpenApiObject();

                foreach (var propertyInfo in propNames)
                {
                    propObj.Add(propertyInfo.Name, new OpenApiString("")); //TODO Could use Bogus to generate some data rather than empty string
                }

                var validatorLocator = context.RequestServices.GetRequiredService<IValidatorLocator>();

                var validator = validatorLocator.GetValidator(requestType);

                var validatorDescriptor = validator.CreateDescriptor();

                var schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = propNames.ToDictionary(key => key.Name, value => new OpenApiSchema { Type = GetOpenApiTypeMapping(value.Type) }),
                    Example = propObj
                };

                //Thanks for the pointers https://github.com/micro-elements/MicroElements.Swashbuckle.FluentValidation
                //TODO Also need to look at request parameters that might be required from the header or querystring for example. MVC does binding on GETs from these sources
                foreach (var key in schema.Properties.Keys)
                {
                    foreach (var propertyValidator in validatorDescriptor
                        .GetValidatorsForMember(ToPascalCase(key)))
                    {
                        if (propertyValidator is INotNullValidator
                            || propertyValidator is INotEmptyValidator)
                        {
                            if (!schema.Required.Contains(key))
                            {
                                schema.Required.Add(key);
                            }

                            if (propertyValidator is INotEmptyValidator)
                            {
                                schema.Properties[key].MinLength = 1;
                            }
                        }

                        if (propertyValidator is ILengthValidator lengthValidator)
                        {
                            if (lengthValidator.Max > 0)
                                schema.Properties[key].MaxLength = lengthValidator.Max;

                            schema.Properties[key].MinLength = lengthValidator.Min;
                        }

                        if (propertyValidator is IComparisonValidator comparisonValidator)
                        {
                            //var comparisonValidator = (IComparisonValidator)context.PropertyValidator;
                            if (comparisonValidator.ValueToCompare.IsNumeric())
                            {
                                var valueToCompare = comparisonValidator.ValueToCompare.NumericToDecimal();
                                var schemaProperty = schema.Properties[key];

                                if (comparisonValidator.Comparison == Comparison.GreaterThanOrEqual)
                                {
                                    schemaProperty.Minimum = valueToCompare;
                                }
                                else if (comparisonValidator.Comparison == Comparison.GreaterThan)
                                {
                                    schemaProperty.Minimum = valueToCompare;
                                    schemaProperty.ExclusiveMinimum = true;
                                }
                                else if (comparisonValidator.Comparison == Comparison.LessThanOrEqual)
                                {
                                    schemaProperty.Maximum = valueToCompare;
                                }
                                else if (comparisonValidator.Comparison == Comparison.LessThan)
                                {
                                    schemaProperty.Maximum = valueToCompare;
                                    schemaProperty.ExclusiveMaximum = true;
                                }
                            }
                        }

                        if (propertyValidator is RegularExpressionValidator expressionValidator)
                        {
                            schema.Properties[key].Pattern = expressionValidator.Expression;
                        }

                        if (propertyValidator is IBetweenValidator betweenValidator)
                        {
                            var schemaProperty = schema.Properties[key];

                            if (betweenValidator.From.IsNumeric())
                            {
                                schemaProperty.Minimum = betweenValidator.From.NumericToDecimal();

                                if (betweenValidator is ExclusiveBetweenValidator)
                                {
                                    schemaProperty.ExclusiveMinimum = true;
                                }
                            }

                            if (betweenValidator.To.IsNumeric())
                            {
                                schemaProperty.Maximum = betweenValidator.To.NumericToDecimal();

                                if (betweenValidator is ExclusiveBetweenValidator)
                                {
                                    schemaProperty.ExclusiveMaximum = true;
                                }
                            }
                        }
                    }
                }

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
                        //Schema = arrayType ? arrayschema : schema
                        Schema = new OpenApiSchema { Reference = new OpenApiReference { Id = requestType.Name, Type = ReferenceType.Schema } }
                    });

                operation.RequestBody = requestBody;

                //TODO Should we document array request bodies and if so the next line applies:
                //TODO Should we check that at the end that any components that are "array" types have a component registered of the  singularTypeName for example you could have IEnumerable<Foo> but Foo is not used in another route so won't be registered in components
                if (!document.Components.Schemas.ContainsKey(requestType.Name))
                {
                    document.Components.Schemas.Add(requestType.Name, schema);
                }
            }
        }

        public static bool IsNumeric(this object value) => value is int || value is long || value is float || value is double || value is decimal;

        /// <summary>
        /// Convert numeric to double.
        /// </summary>
        public static decimal NumericToDecimal(this object value) => Convert.ToDecimal(value);

        private static string ToPascalCase(string inputString)
        {
            // If there are 0 or 1 characters, just return the string.
            if (inputString == null) return null;
            if (inputString.Length < 2) return inputString.ToUpper();
            return inputString.Substring(0, 1).ToUpper() + inputString.Substring(1);
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
