// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// Header expression, The token identifier in header is case-insensitive.
    /// </summary>
    public class HeaderExpression : SourceExpression
    {
        /// <summary>
        /// header. string
        /// </summary>
        public const string Header = "header.";

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderExpression"/> class.
        /// </summary>
        /// <param name="token">The token string, it's case-insensitive.</param>
        public HeaderExpression(string token)
            : base(token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(token));
            }
        }

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression
        {
            get
            {
                return Header + Value;
            }
        }

        /// <summary>
        /// Gets the token string.
        /// </summary>
        public string Token
        {
            get
            {
                return Value;
            }
        }
    }
}