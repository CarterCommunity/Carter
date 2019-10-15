// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace Microsoft.OpenApi.Services
{
    /// <summary>
    /// Class containing logic to get differences between two <see cref="OpenApiDocument"/>s.
    /// </summary>
    public static class OpenApiComparer
    {
        /// <summary>
        /// Compares two <see cref="OpenApiDocument"/>s and returns a list of differences.
        /// </summary>
        public static List<OpenApiDifference> Compare(OpenApiDocument source, OpenApiDocument target)
        {
            var diffs = new List<OpenApiDifference>();
            return diffs;
        }
    }
}