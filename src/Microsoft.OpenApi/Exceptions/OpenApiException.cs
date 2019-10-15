// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Exceptions
{
    /// <summary>
    /// Exception type representing exceptions in the Open API library.
    /// </summary>
    public class OpenApiException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="OpenApiException"/> class with default values.
        /// </summary>
        public OpenApiException()
            : this(SRResource.OpenApiExceptionGenericError)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OpenApiException"/> class with an error message.
        /// </summary>
        /// <param name="message">The plain text error message for this exception.</param>
        public OpenApiException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="OpenApiException"/> class with an error message and an inner exception.
        /// </summary>
        /// <param name="message">The plain text error message for this exception.</param>
        /// <param name="innerException">The inner exception that is the cause of this exception to be thrown.</param>
        public OpenApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// The reference pointer.
        /// </summary>
        public string Pointer { get; set; }
    }
}