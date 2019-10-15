// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.IO;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// A custom <see cref="StreamWriter"/> which supports setting a <see cref="IFormatProvider"/>.
    /// </summary>
    public class FormattingStreamWriter : StreamWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattingStreamWriter"/> class.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="formatProvider"></param>
        public FormattingStreamWriter(Stream stream, IFormatProvider formatProvider)
            : base(stream)
        {
            this.FormatProvider = formatProvider;
        }

        /// <summary>
        /// The <see cref="IFormatProvider"/> associated with this <see cref="FormattingStreamWriter"/>.
        /// </summary>
        public override IFormatProvider FormatProvider { get; }
    }
}
