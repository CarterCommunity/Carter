// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The wrapper either for <see cref="IOpenApiAny"/> or <see cref="RuntimeExpression"/>
    /// </summary>
    public class RuntimeExpressionAnyWrapper : IOpenApiElement
    {
        private IOpenApiAny _any;
        private RuntimeExpression _expression;

        /// <summary>
        /// Gets/Sets the <see cref="IOpenApiAny"/>
        /// </summary>
        public IOpenApiAny Any
        {
            get
            {
                return _any;
            }
            set
            {
                _expression = null;
                _any = value;
            }
        }

        /// <summary>
        /// Gets/Set the <see cref="RuntimeExpression"/>
        /// </summary>
        public RuntimeExpression Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _any = null;
                _expression = value;
            }
        }

        /// <summary>
        /// Write <see cref="RuntimeExpressionAnyWrapper"/>
        /// </summary>
        public void WriteValue(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (_any != null)
            {
                writer.WriteAny(_any);
            }
            else if (_expression != null)
            {
                writer.WriteValue(_expression.Expression);
            }
        }
        
        /// <summary>
        /// Write <see cref="RuntimeExpressionAnyWrapper"/>
        /// </summary>
        public async Task WriteValueAsync(IOpenApiWriter writer)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (_any != null)
            {
                await writer.WriteAnyAsync(_any);
            }
            else if (_expression != null)
            {
                await writer.WriteValueAsync(_expression.Expression);
            }
        }
    }
}