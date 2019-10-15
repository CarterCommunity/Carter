// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API string type.
    /// </summary>
    public class OpenApiString : OpenApiPrimitive<string>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiString"/> class.
        /// </summary>
        /// <param name="value"></param>
        public OpenApiString(string value)
            : base(value)
        {
        }

        /// <summary>
        /// The primitive class this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.String;
    }
}