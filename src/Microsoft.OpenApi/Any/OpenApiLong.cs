// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API long.
    /// </summary>
    public class OpenApiLong : OpenApiPrimitive<long>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiLong"/> class.
        /// </summary>
        public OpenApiLong(long value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Long;
    }
}