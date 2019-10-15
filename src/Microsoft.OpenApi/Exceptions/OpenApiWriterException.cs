// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Exceptions
{
    /// <summary>
    /// Exception type representing exceptions in the OpenAPI writer.
    /// </summary>
    public class OpenApiWriterException : OpenApiException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="OpenApiWriterException"/> class with default values.
        /// </summary>
        public OpenApiWriterException()
            : this(SRResource.OpenApiWriterExceptionGenericError)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OpenApiWriterException"/> class with an error message.
        /// </summary>
        /// <param name="message">The plain text error message for this exception.</param>
        public OpenApiWriterException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OpenApiWriterException"/> class with an error message and an inner exception.
        /// </summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception to be thrown.</param>
        public OpenApiWriterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}