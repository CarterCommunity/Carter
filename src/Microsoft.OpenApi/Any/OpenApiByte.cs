// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API Byte
    /// </summary>
    public class OpenApiByte : OpenApiPrimitive<byte[]>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiByte"/> class.
        /// </summary>
        public OpenApiByte(byte value)
            : this(new byte[] { value })
        {
        }

        /// <summary>
        /// Initializes the <see cref="OpenApiByte"/> class.
        /// </summary>
        public OpenApiByte(byte[] value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Byte;
    }
}