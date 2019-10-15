// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Open API visitor base provides common logic for concrete visitors
    /// </summary>
    public abstract class OpenApiVisitorBase
    {
        private readonly Stack<string> _path = new Stack<string>();

        /// <summary>
        /// Properties available to identify context of where an object is within OpenAPI Document
        /// </summary>
        public CurrentKeys CurrentKeys { get; } = new CurrentKeys();

        /// <summary>
        /// Allow Rule to indicate validation error occured at a deeper context level.  
        /// </summary>
        /// <param name="segment">Identifier for context</param>
        public void Enter(string segment)
        {
            this._path.Push(segment);
        }

        /// <summary>
        /// Exit from path context elevel.  Enter and Exit calls should be matched.
        /// </summary>
        public void Exit()
        {
            this._path.Pop();
        }

        /// <summary>
        /// Pointer to source of validation error in document
        /// </summary>
        public string PathString
        {
            get
            {
                return "#/" + String.Join("/", _path.Reverse());
            }
        }

        /// <summary>
        /// Visits <see cref="OpenApiDocument"/>
        /// </summary>
        public virtual void Visit(OpenApiDocument doc)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiInfo"/>
        /// </summary>
        public virtual void Visit(OpenApiInfo info)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiContact"/>
        /// </summary>
        public virtual void Visit(OpenApiContact contact)
        {
        }


        /// <summary>
        /// Visits <see cref="OpenApiLicense"/>
        /// </summary>
        public virtual void Visit(OpenApiLicense license)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiServer"/>
        /// </summary>
        public virtual void Visit(IList<OpenApiServer> servers)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiServer"/>
        /// </summary>
        public virtual void Visit(OpenApiServer server)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiPaths"/>
        /// </summary>
        public virtual void Visit(OpenApiPaths paths)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiPathItem"/>
        /// </summary>
        public virtual void Visit(OpenApiPathItem pathItem)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiServerVariable"/>
        /// </summary>
        public virtual void Visit(OpenApiServerVariable serverVariable)
        {
        }

        /// <summary>
        /// Visits the operations.
        /// </summary>
        public virtual void Visit(IDictionary<OperationType, OpenApiOperation> operations)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiOperation"/>
        /// </summary>
        public virtual void Visit(OpenApiOperation operation)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiParameter"/>
        /// </summary>
        public virtual void Visit(IList<OpenApiParameter> parameters)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiParameter"/>
        /// </summary>
        public virtual void Visit(OpenApiParameter parameter)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiRequestBody"/>
        /// </summary>
        public virtual void Visit(OpenApiRequestBody requestBody)
        {
        }


        /// <summary>
        /// Visits headers.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiHeader> headers)
        {
        }

        /// <summary>
        /// Visits callbacks.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiCallback> callbacks)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponse"/>
        /// </summary>
        public virtual void Visit(OpenApiResponse response)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiResponses"/>
        /// </summary>
        public virtual void Visit(OpenApiResponses response)
        {
        }

        /// <summary>
        /// Visits media type content.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiMediaType> content)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiMediaType"/>
        /// </summary>
        public virtual void Visit(OpenApiMediaType mediaType)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiEncoding"/>
        /// </summary>
        public virtual void Visit(OpenApiEncoding encoding)
        {
        }

        /// <summary>
        /// Visits the examples.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiExample> examples)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiComponents"/>
        /// </summary>
        public virtual void Visit(OpenApiComponents components)
        {
        }


        /// <summary>
        /// Visits <see cref="OpenApiComponents"/>
        /// </summary>
        public virtual void Visit(OpenApiExternalDocs externalDocs)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiSchema"/>
        /// </summary>
        public virtual void Visit(OpenApiSchema schema)
        {
        }

        /// <summary>
        /// Visits the links.
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiLink> links)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiLink"/>
        /// </summary>
        public virtual void Visit(OpenApiLink link)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiCallback"/>
        /// </summary>
        public virtual void Visit(OpenApiCallback callback)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiTag"/>
        /// </summary>
        public virtual void Visit(OpenApiTag tag)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiHeader"/>
        /// </summary>
        public virtual void Visit(OpenApiHeader tag)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiOAuthFlow"/>
        /// </summary>
        public virtual void Visit(OpenApiOAuthFlow openApiOAuthFlow)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiSecurityRequirement"/>
        /// </summary>
        public virtual void Visit(OpenApiSecurityRequirement securityRequirement)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiSecurityScheme"/>
        /// </summary>
        public virtual void Visit(OpenApiSecurityScheme securityScheme)
        {
        }

        /// <summary>
        /// Visits <see cref="OpenApiExample"/>
        /// </summary>
        public virtual void Visit(OpenApiExample example)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiTag"/>
        /// </summary>
        public virtual void Visit(IList<OpenApiTag> openApiTags)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiSecurityRequirement"/>
        /// </summary>
        public virtual void Visit(IList<OpenApiSecurityRequirement> openApiSecurityRequirements)
        {
        }
        
        /// <summary>
        /// Visits <see cref="IOpenApiExtensible"/>
        /// </summary>
        public virtual void Visit(IOpenApiExtensible openApiExtensible)
        {
        }

        /// <summary>
        /// Visits <see cref="IOpenApiExtension"/>
        /// </summary>
        public virtual void Visit(IOpenApiExtension openApiExtension)
        {
        }

        /// <summary>
        /// Visits list of <see cref="OpenApiExample"/>
        /// </summary>
        public virtual void Visit(IList<OpenApiExample> example)
        {
        }

        /// <summary>
        /// Visits a dictionary of server variables
        /// </summary>
        public virtual void Visit(IDictionary<string, OpenApiServerVariable> serverVariables)
        {
        }

        /// <summary>
        /// Visits a dictionary of encodings
        /// </summary>
        /// <param name="encodings"></param>
        public virtual void Visit(IDictionary<string, OpenApiEncoding> encodings)
        {
        }

        /// <summary>
        /// Visits IOpenApiReferenceable instances that are references and not in components
        /// </summary>
        /// <param name="referenceable">referenced object</param>
        public virtual void Visit(IOpenApiReferenceable referenceable)
        {
        }
    }
}