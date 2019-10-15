// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API Float
    /// </summary>
    public class OpenApiFloat : OpenApiPrimitive<float>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiFloat"/> class.
        /// </summary>
        public OpenApiFloat(float value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Float;
    }
}