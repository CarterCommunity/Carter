// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Constants for the writer.
    /// </summary>
    internal static class WriterConstants
    {
        /// <summary>
        /// JSON datetime format.
        /// </summary>
        internal const string ODataDateTimeFormat = @"\/Date({0})\/";

        /// <summary>
        /// JSON datetime offset format.
        /// </summary>
        internal const string ODataDateTimeOffsetFormat = @"\/Date({0}{1}{2:D4})\/";

        /// <summary>
        /// A plus sign for the date time offset format.
        /// </summary>
        internal const string ODataDateTimeOffsetPlusSign = "+";

        /// <summary>
        /// The true value literal.
        /// </summary>
        internal const string JsonTrueLiteral = "true";

        /// <summary>
        /// The false value literal.
        /// </summary>
        internal const string JsonFalseLiteral = "false";

        /// <summary>
        /// The null value literal.
        /// </summary>
        internal const string JsonNullLiteral = "null";

        /// <summary>
        /// Character which starts the object scope.
        /// </summary>
        internal const string StartObjectScope = "{";

        /// <summary>
        /// Character which ends the object scope.
        /// </summary>
        internal const string EndObjectScope = "}";

        /// <summary>
        /// Character which starts the array scope.
        /// </summary>
        internal const string StartArrayScope = "[";

        /// <summary>
        /// Character which ends the array scope.
        /// </summary>
        internal const string EndArrayScope = "]";

        /// <summary>
        /// "(" Json Padding Function scope open parens.
        /// </summary>
        internal const string StartPaddingFunctionScope = "(";

        /// <summary>
        /// ")" Json Padding Function scope close parens.
        /// </summary>
        internal const string EndPaddingFunctionScope = ")";

        /// <summary>
        /// The separator between object members.
        /// </summary>
        internal const string ObjectMemberSeparator = ",";

        /// <summary>
        /// The separator between array elements.
        /// </summary>
        internal const string ArrayElementSeparator = ",";

        /// <summary>
        /// The separator between the name and the value.
        /// </summary>
        internal const string NameValueSeparator = ": ";

        /// <summary>
        /// The white space for empty object
        /// </summary>
        internal const string WhiteSpaceForEmptyObject = " ";

        /// <summary>
        /// The white space for empty array
        /// </summary>
        internal const string WhiteSpaceForEmptyArray = " ";

        /// <summary>
        /// The prefix of array item
        /// </summary>
        internal const string PrefixOfArrayItem = "- ";

        /// <summary>
        /// The white space for indent
        /// </summary>
        internal const string WhiteSpaceForIndent = "  ";

        /// <summary>
        /// Empty object
        /// </summary>
        /// <remarks>To indicate empty object in YAML.</remarks>
        internal const string EmptyObject = "{ }";

        /// <summary>
        /// Empty array
        /// </summary>
        /// <remarks>To indicate empty array in YAML.</remarks>
        internal const string EmptyArray = "[ ]";
    }
}