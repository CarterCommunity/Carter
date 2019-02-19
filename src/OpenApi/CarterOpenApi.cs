namespace Carter.OpenApi
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
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
                        object singleExample = null;

                        if (valueStatusCode.Response.IsArray() || valueStatusCode.Response.IsCollection() || valueStatusCode.Response.IsEnumerable())
                        {
                            arrayType = true;
                            responseType = valueStatusCode.Response.GetElementType();
                            singleExample = ((IEnumerable<object>)valueStatusCode.ResponseExample)?.FirstOrDefault();

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
                            responseTypeName = responseType.Name.Replace("`1", ""); //If type has a generic constraint remove the `1 from PagedList<Foo>
                        }

                        var propNames = responseType.GetProperties()
                            .Select(x => (Name: x.Name.ToLower(), Type: x.PropertyType.Name.ToLower()))
                            .ToList();

                        var schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = propNames.ToDictionary(key => key.Name, value => new OpenApiSchema { Type = GetOpenApiTypeMapping(value.Type) }),
                            Example = CreateOpenApiExample(responseType, singleExample),
                        };

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
                                    new OpenApiSchema {
                                            Type = "array",
                                            Example = CreateOpenApiExample(valueStatusCode.Response, valueStatusCode.ResponseExample),
                                            Items = new OpenApiSchema { Reference = new OpenApiReference { Id = singularTypeName, Type = ReferenceType.Schema } } });
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
            if (keyValuePair.Key.verb == "GET")
            {
                return;
            }

            if (keyValuePair.Value.Request != null)
            {
                bool arrayType = false;
                Type requestType;
                object singleExample = null;

                if (keyValuePair.Value.Request.IsArray() || keyValuePair.Value.Request.IsCollection() || keyValuePair.Value.Request.IsEnumerable())
                {
                    arrayType = true;
                    requestType = keyValuePair.Value.Request.GetElementType();
                    if (requestType == null)
                    {
                        requestType = keyValuePair.Value.Request.GetGenericArguments().First();
                        singleExample = ((IEnumerable<object>)keyValuePair.Value.RequestExample)?.FirstOrDefault();
                    }
                }
                else
                {
                    requestType = keyValuePair.Value.Request;
                    singleExample = keyValuePair.Value.RequestExample;
                }

                var propNames = requestType.GetProperties()
                    .Select(x => (Name: x.Name.ToLower(), Type: x.PropertyType.Name.ToLower()))
                    .ToList();

                var validatorLocator = context.RequestServices.GetRequiredService<IValidatorLocator>();

                var validator = validatorLocator.GetValidator(requestType);

                var schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = propNames.ToDictionary(key => key.Name, value => new OpenApiSchema { Type = GetOpenApiTypeMapping(value.Type) }),
                    Example = CreateOpenApiExample(requestType, singleExample),
                };

                if (validator != null)
                {
                    var validatorDescriptor = validator.CreateDescriptor();

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
                }

                var requestBody = new OpenApiRequestBody();
                requestBody.Content.Add("application/json",
                    new OpenApiMediaType
                    {
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

        private static IOpenApiAny CreateOpenApiExample(Type exampleType, object exampleInstance) =>
            exampleType == null
                ? null
                : exampleInstance == null
                    ? CreateOpenApiObject(exampleType, true)
                    : CreateOpenApiObject(exampleType, false, exampleInstance);

        private static readonly Dictionary<Type, (Func<object, IOpenApiAny> PrimitiveTypeFactory, object DefaultValue)> OpenApiPrimitiveTypeMappings = new Dictionary<Type, (Func<object, IOpenApiAny> PrimitiveTypeFactory, object DefaultValue)>
        {
            [typeof(bool)] = (x => new OpenApiBoolean((bool)x), false),

            [typeof(int)] = (x => new OpenApiInteger((int)x), 0),
            [typeof(short)] = (x => new OpenApiInteger((int)x), 0),
            [typeof(ushort)] = (x => new OpenApiInteger((int)x), 0),

            [typeof(uint)] = (x => new OpenApiLong((long)x), 0),
            [typeof(long)] = (x => new OpenApiLong((long)x), 0),
            [typeof(ulong)] = (x => new OpenApiLong((long)x), 0),

            [typeof(float)] = (x => new OpenApiFloat((float)x), 0.0f),
            [typeof(double)] = (x => new OpenApiDouble((double)x), 0.0d),
            [typeof(decimal)] = (x => new OpenApiDouble(Convert.ToDouble((decimal)x)), 0.1m),

            [typeof(byte)] = (x => new OpenApiByte((byte)x), (byte)0xFF),

            // Passing an actual byte[] causes an OpenApiWriterException
            // "The type 'System.Byte[]' is not supported in Open API document."
            // For now i've set the default example to null, which works. But this might catch
            // people out if they are providing their own example instances - might need to special
            // case the handling of these?
            [typeof(byte[])] = (x => new OpenApiBinary((byte[])x), null), 
            
            [typeof(string)] = (x => new OpenApiString((string) x), "Example"),
            
            [typeof(DateTime)] = (x => new OpenApiDateTime((DateTime) x), DateTime.Now),
        };

        private static IOpenApiAny CreateOpenApiObject(Type objectType, bool useDefaults, object exampleValue = null)
        {
            if (exampleValue == null && !useDefaults)
            {
                return new OpenApiNull();
            }
            if (OpenApiPrimitiveTypeMappings.TryGetValue(objectType, out var primitiveMapping))
            {
                return primitiveMapping.PrimitiveTypeFactory(useDefaults
                    ? primitiveMapping.DefaultValue
                    : exampleValue);
            }

            if (objectType.IsArray() || objectType.IsCollection() || objectType.IsEnumerable())
            {
                var arrayElementType = objectType.GetElementType() ?? GetEnumerableType(objectType);

                var arrayElements = useDefaults
                    ? (IEnumerable)new List<object>()
                    : (IEnumerable)exampleValue;

                var arrayExample = new OpenApiArray();

                foreach (var element in arrayElements)
                {
                    arrayExample.Add(CreateOpenApiObject(arrayElementType, useDefaults, element));
                }

                return arrayExample;
            }

            var example = new OpenApiObject();

            var properties = objectType.GetProperties();
            foreach (var property in properties)
            {
                object propertyValue = null;
                if (exampleValue != null)
                {
                    propertyValue = property.GetValue(exampleValue);
                }

                // TODO: I Maintained the original functionality of name.tolower here, but camelcase would be nicer I think?
                example[property.Name.ToLower()] = CreateOpenApiObject(property.PropertyType, useDefaults, propertyValue);
            }

            return example;
        }

        public static Type GetEnumerableType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            var iface = (from i in type.GetInterfaces()
                where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                select i).FirstOrDefault();

            if (iface == null)
                return null;

            return GetEnumerableType(iface);
        }
    }
}
