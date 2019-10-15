// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Class containing dispatchers to execute validation rules on for Open API document.
    /// </summary>
    public class OpenApiValidator : OpenApiVisitorBase, IValidationContext 
    {
        private readonly ValidationRuleSet _ruleSet;
        private readonly IList<OpenApiValidatorError> _errors = new List<OpenApiValidatorError>();

        /// <summary>
        /// Create a vistor that will validate an OpenAPIDocument
        /// </summary>
        /// <param name="ruleSet"></param>
        public OpenApiValidator(ValidationRuleSet ruleSet) 
        {
            _ruleSet = ruleSet;
        }
        
        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        public IEnumerable<OpenApiValidatorError> Errors
        {
            get
            {
                return _errors;
            }
        }

        /// <summary>
        /// Register an error with the validation context.
        /// </summary>
        /// <param name="error">Error to register.</param>
        public void AddError(OpenApiValidatorError error)
        {
            if (error == null)
            {
                throw Error.ArgumentNull(nameof(error));
            }

            _errors.Add(error);
        }


        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiDocument"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiDocument item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiInfo"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiInfo item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiContact"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiContact item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiComponents"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiComponents item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiHeader"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiHeader item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiResponse"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiResponse item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiMediaType"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiMediaType item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiResponses"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiResponses item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiExternalDocs"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiExternalDocs item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiLicense"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiLicense item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiOAuthFlow"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiOAuthFlow item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiTag"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiTag item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiParameter"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiParameter item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiSchema"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiSchema item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiServer"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiServer item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiEncoding"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiEncoding item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="OpenApiCallback"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(OpenApiCallback item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="IOpenApiExtensible"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IOpenApiExtensible item) => Validate(item);

        /// <summary>
        /// Execute validation rules against an <see cref="IOpenApiExtension"/>
        /// </summary>
        /// <param name="item">The object to be validated</param>
        public override void Visit(IOpenApiExtension item) => Validate(item, item.GetType());

        /// <summary>
        /// Execute validation rules against a list of <see cref="OpenApiExample"/>
        /// </summary>
        /// <param name="items">The object to be validated</param>
        public override void Visit(IList<OpenApiExample> items) => Validate(items, items.GetType());

        private void Validate<T>(T item)
        {
            var type = typeof(T);

            Validate(item, type);
        }

        /// <summary>
        /// This overload allows applying rules based on actual object type, rather than matched interface.  This is 
        /// needed for validating extensions.
        /// </summary>
        private void Validate(object item, Type type)
        {
            if (item == null)
            {
                return;  // Required fields should be checked by higher level objects
            }

            // Validate unresolved references as references
            var potentialReference = item as IOpenApiReferenceable;
            if (potentialReference != null && potentialReference.UnresolvedReference)
            {
                type = typeof(IOpenApiReferenceable);  
            }

            var rules = _ruleSet.FindRules(type);
            foreach (var rule in rules)
            {
                rule.Evaluate(this as IValidationContext, item);
            }
        }
    }
}