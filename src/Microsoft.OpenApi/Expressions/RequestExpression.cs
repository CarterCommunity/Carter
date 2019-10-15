// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// $request. expression.
    /// </summary>
    public sealed class RequestExpression : RuntimeExpression
    {
        /// <summary>
        /// $request. string
        /// </summary>
        public const string Request = "$request.";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExpression"/> class.
        /// </summary>
        /// <param name="source">The source of the request.</param>
        public RequestExpression(SourceExpression source)
        {
            Source = source ?? throw Error.ArgumentNull(nameof(source));
        }

        /// <summary>
        /// Gets the expression string.
        /// </summary>
        public override string Expression => Request + Source.Expression;

        /// <summary>
        /// The <see cref="SourceExpression"/> expression.
        /// </summary>
        public SourceExpression Source { get; }
    }
}
