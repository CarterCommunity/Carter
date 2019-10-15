// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Validations
{
    /// <summary>
    /// Class containing validation rule logic.
    /// </summary>
    public abstract class ValidationRule
    {
        /// <summary>
        /// Element Type.
        /// </summary>
        internal abstract Type ElementType { get; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The object item.</param>
        internal abstract void Evaluate(IValidationContext context, object item);
    }

    /// <summary>
    /// Class containing validation rule logic for <see cref="IOpenApiElement"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValidationRule<T> : ValidationRule where T: IOpenApiElement
    {
        private readonly Action<IValidationContext, T> _validate;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRule"/> class.
        /// </summary>
        /// <param name="validate">Action to perform the validation.</param>
        public ValidationRule(Action<IValidationContext, T> validate)
        {
            _validate = validate ?? throw Error.ArgumentNull(nameof(validate));
        }

        internal override Type ElementType
        {
            get { return typeof(T); }
        }

        internal override void Evaluate(IValidationContext context, object item)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (item == null)
            {
                return;
            }

            if (!(item is T))
            {
                throw Error.Argument(string.Format(SRResource.InputItemShouldBeType, typeof(T).FullName));
            }

            T typedItem = (T)item;
            this._validate(context, typedItem);
        }
    }
}
