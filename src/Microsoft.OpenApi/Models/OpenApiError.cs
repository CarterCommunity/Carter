// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Exceptions;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// Error related to the Open API Document.
    /// </summary>
    public class OpenApiError
    {
        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class using the message and pointer from the given exception.
        /// </summary>
        public OpenApiError(OpenApiException exception) : this(exception.Pointer, exception.Message)
        {
        }

        /// <summary>
        /// Initializes the <see cref="OpenApiError"/> class.
        /// </summary>
        public OpenApiError(string pointer, string message)
        {
            Pointer = pointer;
            Message = message;
        }

        /// <summary>
        /// Message explaining the error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Pointer to the location of the error.
        /// </summary>
        public string Pointer { get; set; }

        /// <summary>
        /// Gets the string representation of <see cref="OpenApiError"/>.
        /// </summary>
        public override string ToString()
        {
            return Message + (!string.IsNullOrEmpty(Pointer) ? " [" + Pointer + "]" : "" );
        }
    }
}