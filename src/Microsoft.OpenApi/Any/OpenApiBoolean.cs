// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API boolean.
    /// </summary>
    public class OpenApiBoolean : OpenApiPrimitive<bool>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiBoolean"/> class.
        /// </summary>
        /// <param name="value"></param>
        public OpenApiBoolean(bool value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Boolean;
    }
}