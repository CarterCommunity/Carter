// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API Date
    /// </summary>
    public class OpenApiDate : OpenApiPrimitive<DateTime>
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiDate"/> class.
        /// </summary>
        public OpenApiDate(DateTime value)
            : base(value)
        {
        }

        /// <summary>
        /// Primitive type this object represents.
        /// </summary>
        public override PrimitiveType PrimitiveType { get; } = PrimitiveType.Date;
    }
}