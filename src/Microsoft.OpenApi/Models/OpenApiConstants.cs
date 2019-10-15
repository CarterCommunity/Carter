// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Constants used in the Open API document.
    /// </summary>
    public static class OpenApiConstants
    {
        /// <summary>
        /// Field: OpenApi
        /// </summary>
        public const string OpenApi = "openapi";

        /// <summary>
        /// Field: Info
        /// </summary>
        public const string Info = "info";

        /// <summary>
        /// Field: Title
        /// </summary>
        public const string Title = "title";

        /// <summary>
        /// Field: Type
        /// </summary>
        public const string Type = "type";

        /// <summary>
        /// Field: Format
        /// </summary>
        public const string Format = "format";

        /// <summary>
        /// Field: Version
        /// </summary>
        public const string Version = "version";

        /// <summary>
        /// Field: Contact
        /// </summary>
        public const string Contact = "contact";

        /// <summary>
        /// Field: License
        /// </summary>
        public const string License = "license";

        /// <summary>
        /// Field: TermsOfService
        /// </summary>
        public const string TermsOfService = "termsOfService";

        /// <summary>
        /// Field: Servers
        /// </summary>
        public const string Servers = "servers";

        /// <summary>
        /// Field: Server
        /// </summary>
        public const string Server = "server";

        /// <summary>
        /// Field: Paths
        /// </summary>
        public const string Paths = "paths";

        /// <summary>
        /// Field: Components
        /// </summary>
        public const string Components = "components";

        /// <summary>
        /// Field: Security
        /// </summary>
        public const string Security = "security";

        /// <summary>
        /// Field: Tags
        /// </summary>
        public const string Tags = "tags";

        /// <summary>
        /// Field: ExternalDocs
        /// </summary>
        public const string ExternalDocs = "externalDocs";

        /// <summary>
        /// Field: OperationRef
        /// </summary>
        public const string OperationRef = "operationRef";

        /// <summary>
        /// Field: OperationId
        /// </summary>
        public const string OperationId = "operationId";

        /// <summary>
        /// Field: Parameters
        /// </summary>
        public const string Parameters = "parameters";

        /// <summary>
        /// Field: RequestBody
        /// </summary>
        public const string RequestBody = "requestBody";

        /// <summary>
        /// Field: ExtensionFieldNamePrefix
        /// </summary>
        public const string ExtensionFieldNamePrefix = "x-";

        /// <summary>
        /// Field: Name
        /// </summary>
        public const string Name = "name";

        /// <summary>
        /// Field: Namespace
        /// </summary>
        public const string Namespace = "namespace";

        /// <summary>
        /// Field: Prefix
        /// </summary>
        public const string Prefix = "prefix";

        /// <summary>
        /// Field: Attribute
        /// </summary>
        public const string Attribute = "attribute";

        /// <summary>
        /// Field: Wrapped
        /// </summary>
        public const string Wrapped = "wrapped";

        /// <summary>
        /// Field: In
        /// </summary>
        public const string In = "in";

        /// <summary>
        /// Field: Summary
        /// </summary>
        public const string Summary = "summary";

        /// <summary>
        /// Field: Variables
        /// </summary>
        public const string Variables = "variables";

        /// <summary>
        /// Field: Description
        /// </summary>
        public const string Description = "description";

        /// <summary>
        /// Field: Required
        /// </summary>
        public const string Required = "required";

        /// <summary>
        /// Field: Deprecated
        /// </summary>
        public const string Deprecated = "deprecated";

        /// <summary>
        /// Field: Style
        /// </summary>
        public const string Style = "style";

        /// <summary>
        /// Field: Explode
        /// </summary>
        public const string Explode = "explode";

        /// <summary>
        /// Field: AllowReserved
        /// </summary>
        public const string AllowReserved = "allowReserved";

        /// <summary>
        /// Field: Schema
        /// </summary>
        public const string Schema = "schema";

        /// <summary>
        /// Field: Schemas
        /// </summary>
        public const string Schemas = "schemas";

        /// <summary>
        /// Field: Responses
        /// </summary>
        public const string Responses = "responses";

        /// <summary>
        /// Field: Example
        /// </summary>
        public const string Example = "example";

        /// <summary>
        /// Field: Examples
        /// </summary>
        public const string Examples = "examples";

        /// <summary>
        /// Field: Encoding
        /// </summary>
        public const string Encoding = "encoding";

        /// <summary>
        /// Field: RequestBodies
        /// </summary>
        public const string RequestBodies = "requestBodies";

        /// <summary>
        /// Field: AllowEmptyValue
        /// </summary>
        public const string AllowEmptyValue = "allowEmptyValue";

        /// <summary>
        /// Field: Value
        /// </summary>
        public const string Value = "value";

        /// <summary>
        /// Field: ExternalValue
        /// </summary>
        public const string ExternalValue = "externalValue";

        /// <summary>
        /// Field: DollarRef
        /// </summary>
        public const string DollarRef = "$ref";

        /// <summary>
        /// Field: Headers
        /// </summary>
        public const string Headers = "headers";

        /// <summary>
        /// Field: SecuritySchemes
        /// </summary>
        public const string SecuritySchemes = "securitySchemes";

        /// <summary>
        /// Field: Content
        /// </summary>
        public const string Content = "content";

        /// <summary>
        /// Field: Links
        /// </summary>
        public const string Links = "links";

        /// <summary>
        /// Field: Callbacks
        /// </summary>
        public const string Callbacks = "callbacks";

        /// <summary>
        /// Field: Url
        /// </summary>
        public const string Url = "url";

        /// <summary>
        /// Field: Email
        /// </summary>
        public const string Email = "email";

        /// <summary>
        /// Field: Default
        /// </summary>
        public const string Default = "default";

        /// <summary>
        /// Field: Enum
        /// </summary>
        public const string Enum = "enum";

        /// <summary>
        /// Field: MultipleOf
        /// </summary>
        public const string MultipleOf = "multipleOf";

        /// <summary>
        /// Field: Maximum
        /// </summary>
        public const string Maximum = "maximum";

        /// <summary>
        /// Field: ExclusiveMaximum
        /// </summary>
        public const string ExclusiveMaximum = "exclusiveMaximum";

        /// <summary>
        /// Field: Minimum
        /// </summary>
        public const string Minimum = "minimum";

        /// <summary>
        /// Field: ExclusiveMinimum
        /// </summary>
        public const string ExclusiveMinimum = "exclusiveMinimum";

        /// <summary>
        /// Field: MaxLength
        /// </summary>
        public const string MaxLength = "maxLength";

        /// <summary>
        /// Field: MinLength
        /// </summary>
        public const string MinLength = "minLength";

        /// <summary>
        /// Field: Pattern
        /// </summary>
        public const string Pattern = "pattern";

        /// <summary>
        /// Field: MaxItems
        /// </summary>
        public const string MaxItems = "maxItems";

        /// <summary>
        /// Field: MinItems
        /// </summary>
        public const string MinItems = "minItems";

        /// <summary>
        /// Field: UniqueItems
        /// </summary>
        public const string UniqueItems = "uniqueItems";

        /// <summary>
        /// Field: MaxProperties
        /// </summary>
        public const string MaxProperties = "maxProperties";

        /// <summary>
        /// Field: MinProperties
        /// </summary>
        public const string MinProperties = "minProperties";

        /// <summary>
        /// Field: AllOf
        /// </summary>
        public const string AllOf = "allOf";

        /// <summary>
        /// Field: OneOf
        /// </summary>
        public const string OneOf = "oneOf";

        /// <summary>
        /// Field: AnyOf
        /// </summary>
        public const string AnyOf = "anyOf";

        /// <summary>
        /// Field: Not
        /// </summary>
        public const string Not = "not";

        /// <summary>
        /// Field: Items
        /// </summary>
        public const string Items = "items";

        /// <summary>
        /// Field: Properties
        /// </summary>
        public const string Properties = "properties";

        /// <summary>
        /// Field: AdditionalProperties
        /// </summary>
        public const string AdditionalProperties = "additionalProperties";

        /// <summary>
        /// Field: Nullable
        /// </summary>
        public const string Nullable = "nullable";

        /// <summary>
        /// Field: Discriminator
        /// </summary>
        public const string Discriminator = "discriminator";

        /// <summary>
        /// Field: ReadOnly
        /// </summary>
        public const string ReadOnly = "readOnly";

        /// <summary>
        /// Field: WriteOnly
        /// </summary>
        public const string WriteOnly = "writeOnly";

        /// <summary>
        /// Field: Xml
        /// </summary>
        public const string Xml = "xml";

        /// <summary>
        /// Field: Flow
        /// </summary>
        public const string Flow = "flow";

        /// <summary>
        /// Field: Application
        /// </summary>
        public const string Application = "application";

        /// <summary>
        /// Field: AccessCode
        /// </summary>
        public const string AccessCode = "accessCode";

        /// <summary>
        /// Field: Implicit
        /// </summary>
        public const string Implicit = "implicit";

        /// <summary>
        /// Field: Password
        /// </summary>
        public const string Password = "password";

        /// <summary>
        /// Field: ClientCredentials
        /// </summary>
        public const string ClientCredentials = "clientCredentials";

        /// <summary>
        /// Field: AuthorizationCode
        /// </summary>
        public const string AuthorizationCode = "authorizationCode";

        /// <summary>
        /// Field: AuthorizationUrl
        /// </summary>
        public const string AuthorizationUrl = "authorizationUrl";

        /// <summary>
        /// Field: TokenUrl
        /// </summary>
        public const string TokenUrl = "tokenUrl";

        /// <summary>
        /// Field: RefreshUrl
        /// </summary>
        public const string RefreshUrl = "refreshUrl";

        /// <summary>
        /// Field: Scopes
        /// </summary>
        public const string Scopes = "scopes";

        /// <summary>
        /// Field: ContentType
        /// </summary>
        public const string ContentType = "contentType";

        /// <summary>
        /// Field: Get
        /// </summary>
        public const string Get = "get";

        /// <summary>
        /// Field: Put
        /// </summary>
        public const string Put = "put";

        /// <summary>
        /// Field: Post
        /// </summary>
        public const string Post = "post";

        /// <summary>
        /// Field: Delete
        /// </summary>
        public const string Delete = "delete";

        /// <summary>
        /// Field: Options
        /// </summary>
        public const string Options = "options";

        /// <summary>
        /// Field: Head
        /// </summary>
        public const string Head = "head";

        /// <summary>
        /// Field: Patch
        /// </summary>
        public const string Patch = "patch";

        /// <summary>
        /// Field: Trace
        /// </summary>
        public const string Trace = "trace";

        /// <summary>
        /// Field: PropertyName
        /// </summary>
        public const string PropertyName = "propertyName";

        /// <summary>
        /// Field: Mapping
        /// </summary>
        public const string Mapping = "mapping";

        /// <summary>
        /// Field: Scheme
        /// </summary>
        public const string Scheme = "scheme";

        /// <summary>
        /// Field: BearerFormat
        /// </summary>
        public const string BearerFormat = "bearerFormat";

        /// <summary>
        /// Field: Flows
        /// </summary>
        public const string Flows = "flows";

        /// <summary>
        /// Field: OpenIdConnectUrl
        /// </summary>
        public const string OpenIdConnectUrl = "openIdConnectUrl";

        /// <summary>
        /// Field: DefaultName
        /// </summary>
        public const string DefaultName = "Default Name";

        /// <summary>
        /// Field: DefaultDefault
        /// </summary>
        public const string DefaultDefault = "Default Default";

        /// <summary>
        /// Field: DefaultTitle
        /// </summary>
        public const string DefaultTitle = "Default Title";

        /// <summary>
        /// Field: DefaultDescription
        /// </summary>
        public const string DefaultDescription = "Default Description";

        /// <summary>
        /// Field: version3_0_0
        /// </summary>
        public static readonly Version version3_0_0 = new Version(3, 0, 0);

        /// <summary>
        /// Field: defaultUrl
        /// </summary>
        public static readonly Uri defaultUrl = new Uri("http://localhost/");

        #region V2.0

        /// <summary>
        /// Field: Host
        /// </summary>
        public const string Host = "host";

        /// <summary>
        /// Field: Swagger
        /// </summary>
        public const string Swagger = "swagger";

        /// <summary>
        /// Field: version2_0
        /// </summary>
        public static readonly Version version2_0 = new Version(2, 0);

        /// <summary>
        /// Field: BasePath
        /// </summary>
        public const string BasePath = "basePath";

        /// <summary>
        /// Field: Schemes
        /// </summary>
        public const string Schemes = "schemes";

        /// <summary>
        /// Field: SecurityDefinitions
        /// </summary>
        public const string SecurityDefinitions = "securityDefinitions";

        /// <summary>
        /// Field: Definitions
        /// </summary>
        public const string Definitions = "definitions";

        /// <summary>
        /// Field: Basic
        /// </summary>
        public const string Basic = "basic";

        /// <summary>
        /// Field: Consumes
        /// </summary>
        public const string Consumes = "consumes";

        /// <summary>
        /// Field: Produces
        /// </summary>
        public const string Produces = "produces";

        #endregion
    }
}