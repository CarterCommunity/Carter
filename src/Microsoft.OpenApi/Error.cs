// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Globalization;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// Utility class for creating and unwrapping <see cref="Exception"/> instances.
    /// </summary>
    internal static class Error
    {
        /// <summary>
        /// Formats the specified resource string using <see cref="M:CultureInfo.CurrentCulture"/>.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The formatted string.</returns>
        internal static string Format(string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        /// Creates an <see cref="ArgumentException"/> with the provided properties.
        /// </summary>
        /// <param name="messageFormat">A composite format string explaining the reason for the exception.</param>
        /// <param name="messageArgs">An object array that contains zero or more objects to format.</param>
        /// <returns>The logged <see cref="Exception"/>.</returns>
        internal static ArgumentException Argument(string messageFormat, params object[] messageArgs)
        {
            return new ArgumentException(Format(messageFormat, messageArgs));
        }

        /// <summary>
        /// Creates an <see cref="ArgumentException"/> with the provided properties.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <param name="messageFormat">A composite format string explaining the reason for the exception.</param>
        /// <param name="messageArgs">An object array that contains zero or more objects to format.</param>
        /// <returns>The logged <see cref="Exception"/>.</returns>
        internal static ArgumentException Argument(
            string parameterName,
            string messageFormat,
            params object[] messageArgs)
        {
            return new ArgumentException(Format(messageFormat, messageArgs), parameterName);
        }

        /// <summary>
        /// Creates an <see cref="ArgumentNullException"/> with the provided properties.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <returns>The logged <see cref="Exception"/>.</returns>
        internal static ArgumentNullException ArgumentNull(string parameterName)
        {
            return new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Creates an <see cref="ArgumentNullException"/> with the provided properties.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <param name="messageFormat">A composite format string explaining the reason for the exception.</param>
        /// <param name="messageArgs">An object array that contains zero or more objects to format.</param>
        /// <returns>The logged <see cref="Exception"/>.</returns>
        internal static ArgumentNullException ArgumentNull(
            string parameterName,
            string messageFormat,
            params object[] messageArgs)
        {
            return new ArgumentNullException(parameterName, Format(messageFormat, messageArgs));
        }

        /// <summary>
        /// Creates an <see cref="ArgumentException"/> with a default message.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>
        /// <returns>The logged <see cref="Exception"/>.</returns>
        internal static ArgumentException ArgumentNullOrWhiteSpace(string parameterName)
        {
            return Argument(parameterName, SRResource.ArgumentNullOrWhiteSpace, parameterName);
        }

        /// <summary>
        /// Creates an <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="messageFormat">A composite format string explaining the reason for the exception.</param>
        /// <param name="messageArgs">An object array that contains zero or more objects to format.</param>
        /// <returns>The logged <see cref="Exception"/>.</returns>
        internal static NotSupportedException NotSupported(string messageFormat, params object[] messageArgs)
        {
            return new NotSupportedException(Format(messageFormat, messageArgs));
        }
    }
}