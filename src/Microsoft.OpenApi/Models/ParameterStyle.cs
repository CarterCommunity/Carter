// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using Microsoft.OpenApi.Attributes;

namespace Microsoft.OpenApi.Models
{
    /// <summary>
    /// The style of the parameter.
    /// </summary>
    public enum ParameterStyle
    {
        /// <summary>
        /// Path-style parameters.
        /// </summary>
        [Display("matrix")] Matrix,

        /// <summary>
        /// Label style parameters.
        /// </summary>
        [Display("label")] Label,

        /// <summary>
        /// Form style parameters.
        /// </summary>
        [Display("form")] Form,

        /// <summary>
        /// Simple style parameters.
        /// </summary>
        [Display("simple")] Simple,

        /// <summary>
        /// Space separated array values.
        /// </summary>
        [Display("spaceDelimited")] SpaceDelimited,

        /// <summary>
        /// Pipe separated array values.
        /// </summary>
        [Display("pipeDelimited")] PipeDelimited,

        /// <summary>
        /// Provides a simple way of rendering nested objects using form parameters.
        /// </summary>
        [Display("deepObject")] DeepObject
    }
}