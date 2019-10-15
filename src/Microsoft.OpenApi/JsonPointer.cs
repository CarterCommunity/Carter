// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Linq;

namespace Microsoft.OpenApi
{
    /// <summary>
    /// JSON pointer.
    /// </summary>
    public class JsonPointer
    {
        /// <summary>
        /// Initializes the <see cref="JsonPointer"/> class.
        /// </summary>
        /// <param name="pointer">Pointer as string.</param>
        public JsonPointer(string pointer)
        {
            Tokens = pointer.Split('/').Skip(1).Select(Decode).ToArray();
        }

        /// <summary>
        /// Initializes the <see cref="JsonPointer"/> class.
        /// </summary>
        /// <param name="tokens">Pointer as tokenized string.</param>
        private JsonPointer(string[] tokens)
        {
            Tokens = tokens;
        }

        /// <summary>
        /// Tokens.
        /// </summary>
        public string[] Tokens { get; }

        /// <summary>
        /// Gets the parent pointer.
        /// </summary>
        public JsonPointer ParentPointer
        {
            get
            {
                if (Tokens.Length == 0)
                {
                    return null;
                }

                return new JsonPointer(Tokens.Take(Tokens.Length - 1).ToArray());
            }
        }

        /// <summary>
        /// Decode the string.
        /// </summary>
        private string Decode(string token)
        {
            return Uri.UnescapeDataString(token).Replace("~1", "/").Replace("~0", "~");
        }

        /// <summary>
        /// Gets the string representation of this JSON pointer.
        /// </summary>
        public override string ToString()
        {
            return "/" + string.Join("/", Tokens);
        }
    }
}