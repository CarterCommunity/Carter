// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API password.
    /// </summary>
    public class OpenApiPassword : OpenApiPrimitive<string>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiPassword"/> class.
        /// </summary>
        public OpenApiPassword(string value)
            : base(value)
        {
        }

        /// <summary>
        /// The primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Password;
    }
}