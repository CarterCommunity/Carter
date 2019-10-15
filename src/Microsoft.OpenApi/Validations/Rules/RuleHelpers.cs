// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Validations.Rules
{
    internal static class RuleHelpers
    {
        internal const string DataTypeMismatchedErrorMessage = "Data and type mismatch found.";

        /// <summary>
        /// Input string must be in the format of an email address
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>True if it's an email address. Otherwise False.</returns>
        public static bool IsEmailAddress(this string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            var splits = input.Split('@');
            if (splits.Length != 2)
            {
                return false;
            }

            if (String.IsNullOrEmpty(splits[0]) || String.IsNullOrEmpty(splits[1]))
            {
                return false;
            }

            // Add more rules.

            return true;
        }

        public static void ValidateDataTypeMismatch(
            IValidationContext context,
            string ruleName,
            IOpenApiAny value,
            OpenApiSchema schema)
        {
            if (schema == null)
            {
                return;
            }

            var type = schema.Type;
            var format = schema.Format;
            var nullable = schema.Nullable;

            // Before checking the type, check first if the schema allows null.
            // If so and the data given is also null, this is allowed for any type.
            if (nullable)
            {
                if (value is OpenApiNull)
                {
                    return;
                }
            }

            if (type == "object")
            {
                // It is not against the spec to have a string representing an object value.
                // To represent examples of media types that cannot naturally be represented in JSON or YAML,
                // a string value can contain the example with escaping where necessary
                if (value is OpenApiString)
                {
                    return;
                }

                // If value is not a string and also not an object, there is a data mismatch.
                if (!(value is OpenApiObject))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                    return;
                }

                var anyObject = (OpenApiObject)value;

                foreach (var key in anyObject.Keys)
                {
                    context.Enter(key);

                    if (schema.Properties != null && schema.Properties.ContainsKey(key))
                    {
                        ValidateDataTypeMismatch(context, ruleName, anyObject[key], schema.Properties[key]);
                    }
                    else
                    {
                        ValidateDataTypeMismatch(context, ruleName, anyObject[key], schema.AdditionalProperties);
                    }

                    context.Exit();
                }

                return;
            }

            if (type == "array")
            {
                // It is not against the spec to have a string representing an array value.
                // To represent examples of media types that cannot naturally be represented in JSON or YAML,
                // a string value can contain the example with escaping where necessary
                if (value is OpenApiString)
                {
                    return;
                }

                // If value is not a string and also not an array, there is a data mismatch.
                if (!(value is OpenApiArray))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                    return;
                }

                var anyArray = (OpenApiArray)value;

                for (int i = 0; i < anyArray.Count; i++)
                {
                    context.Enter(i.ToString());

                    ValidateDataTypeMismatch(context, ruleName, anyArray[i], schema.Items);

                    context.Exit();
                }

                return;
            }

            if (type == "integer" && format == "int32")
            {
                if (!(value is OpenApiInteger))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "integer" && format == "int64")
            {
                if (!(value is OpenApiLong))
                {
                    context.CreateError(
                       ruleName,
                       DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "integer" && !(value is OpenApiInteger))
            {
                if (!(value is OpenApiInteger))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "number" && format == "float")
            {
                if (!(value is OpenApiFloat))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "number" && format == "double")
            {
                if (!(value is OpenApiDouble))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "number")
            {
                if (!(value is OpenApiDouble))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "byte")
            {
                if (!(value is OpenApiByte))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "date")
            {
                if (!(value is OpenApiDate))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "date-time")
            {
                if (!(value is OpenApiDateTime))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string" && format == "password")
            {
                if (!(value is OpenApiPassword))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "string")
            {
                if (!(value is OpenApiString))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }

            if (type == "boolean")
            {
                if (!(value is OpenApiBoolean))
                {
                    context.CreateError(
                        ruleName,
                        DataTypeMismatchedErrorMessage);
                }

                return;
            }
        }
    }
}
