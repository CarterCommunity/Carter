// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API Datetime
    /// </summary>
    public class OpenApiDateTime : OpenApiPrimitive<DateTimeOffset>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiDateTime"/> class.
        /// </summary>
        public OpenApiDateTime(DateTimeOffset value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.DateTime;
    }
}