// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.OpenApi.Expressions
{
    /// <summary>
    /// String literal with embedded expressions
    /// </summary>
    public class CompositeExpression : RuntimeExpression
    {
        private readonly string template;
        private Regex expressionPattern = new Regex(@"{(?<exp>\$[^}]*)");

        /// <summary>
        /// Expressions embedded into string literal
        /// </summary>
        public List<RuntimeExpression> ContainedExpressions = new List<RuntimeExpression>();

        /// <summary>
        /// Create a composite expression from a string literal with an embedded expression
        /// </summary>
        /// <param name="expression"></param>
        public CompositeExpression(string expression)
        {
            template = expression;

            // Extract subexpressions and convert to RuntimeExpressions
            var matches = expressionPattern.Matches(expression);

            foreach (var item in matches.Cast<Match>())
            {
                var value = item.Groups["exp"].Captures.Cast<Capture>().First().Value;
                ContainedExpressions.Add(RuntimeExpression.Build(value));
            }
        }

        /// <summary>
        /// Return original string literal with embedded expression
        /// </summary>
        public override string Expression => template;
    }
}
