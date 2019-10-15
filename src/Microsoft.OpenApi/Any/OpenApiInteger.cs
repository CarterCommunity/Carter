// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API Integer
    /// </summary>
    public class OpenApiInteger : OpenApiPrimitive<int>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiInteger"/> class.
        /// </summary>
        public OpenApiInteger(int value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Integer;
    }
}