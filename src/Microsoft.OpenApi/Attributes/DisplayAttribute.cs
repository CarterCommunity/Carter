// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;

namespace Microsoft.OpenApi.Attributes
{
    /// <summary>
    /// Represents the Open Api Data type metadata attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    internal class DisplayAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAttribute"/> class.
        /// </summary>
        /// <param name="name">The display name.</param>
        public DisplayAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }

            Name = name;
        }

        /// <summary>
        /// The display Name.
        /// </summary>
        public string Name { get; }
    }
}