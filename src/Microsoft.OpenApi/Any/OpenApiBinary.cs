// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API binary.
    /// </summary>
    public class OpenApiBinary : OpenApiPrimitive<byte[]>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiBinary"/> class.
        /// </summary>
        /// <param name="value"></param>
        public OpenApiBinary(byte[] value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Binary;
    }
}