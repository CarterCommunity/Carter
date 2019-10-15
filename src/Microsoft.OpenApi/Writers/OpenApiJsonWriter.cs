// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// JSON Writer.
    /// </summary>
    public class OpenApiJsonWriter : OpenApiWriterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiJsonWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        public OpenApiJsonWriter(TextWriter textWriter) : base(textWriter)
        {
        }

        /// <summary>
        /// Base Indentation Level.
        /// This denotes how many indentations are needed for the property in the base object.
        /// </summary>
        protected override int BaseIndentation => 1;

        /// <summary>
        /// Write JSON start object.
        /// </summary>
        public override void WriteStartObject()
        {
            var previousScope = CurrentScope();

            var currentScope = StartScope(ScopeType.Object);

            if (previousScope != null && previousScope.Type == ScopeType.Array)
            {
                currentScope.IsInArray = true;

                if (previousScope.ObjectCount != 1)
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                Writer.WriteLine();
                WriteIndentation();
            }

            Writer.Write(WriterConstants.StartObjectScope);

            IncreaseIndentation();
        }

        /// <summary>
        /// Write JSON start object.
        /// </summary>
        public override async Task WriteStartObjectAsync()
        {
            var previousScope = CurrentScope();

            var currentScope = StartScope(ScopeType.Object);

            if (previousScope != null && previousScope.Type == ScopeType.Array)
            {
                currentScope.IsInArray = true;

                if (previousScope.ObjectCount != 1)
                {
                    await Writer.WriteAsync(WriterConstants.ArrayElementSeparator);
                }

                await Writer.WriteLineAsync();
                await WriteIndentationAsync();
            }

            await Writer.WriteAsync(WriterConstants.StartObjectScope);

            IncreaseIndentation();
        }


        /// <summary>
        /// Write JSON end object.
        /// </summary>
        public override void WriteEndObject()
        {
            var currentScope = EndScope(ScopeType.Object);
            if (currentScope.ObjectCount != 0)
            {
                Writer.WriteLine();
                DecreaseIndentation();
                WriteIndentation();
            }
            else
            {
                Writer.Write(WriterConstants.WhiteSpaceForEmptyObject);
                DecreaseIndentation();
            }

            Writer.Write(WriterConstants.EndObjectScope);
        }

        /// <summary>
        /// Write JSON end object.
        /// </summary>
        public override async Task WriteEndObjectAsync()
        {
            var currentScope = EndScope(ScopeType.Object);
            if (currentScope.ObjectCount != 0)
            {
                await Writer.WriteLineAsync();
                DecreaseIndentation();
                await WriteIndentationAsync();
            }
            else
            {
                await Writer.WriteAsync(WriterConstants.WhiteSpaceForEmptyObject);
                DecreaseIndentation();
            }

            await Writer.WriteAsync(WriterConstants.EndObjectScope);
        }


        /// <summary>
        /// Write JSON start array.
        /// </summary>
        public override void WriteStartArray()
        {
            var previousScope = CurrentScope();

            var currentScope = StartScope(ScopeType.Array);

            if (previousScope != null && previousScope.Type == ScopeType.Array)
            {
                currentScope.IsInArray = true;

                if (previousScope.ObjectCount != 1)
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                Writer.WriteLine();
                WriteIndentation();
            }

            Writer.Write(WriterConstants.StartArrayScope);
            IncreaseIndentation();
        }

        /// <summary>
        /// Write JSON start array.
        /// </summary>
        public override async Task WriteStartArrayAsync()
        {
            var previousScope = CurrentScope();

            var currentScope = StartScope(ScopeType.Array);

            if (previousScope != null && previousScope.Type == ScopeType.Array)
            {
                currentScope.IsInArray = true;

                if (previousScope.ObjectCount != 1)
                {
                    await Writer.WriteAsync(WriterConstants.ArrayElementSeparator);
                }

                await Writer.WriteLineAsync();
                await WriteIndentationAsync();
            }

            await Writer.WriteAsync(WriterConstants.StartArrayScope);
            IncreaseIndentation();
        }

        /// <summary>
        /// Write JSON end array.
        /// </summary>
        public override void WriteEndArray()
        {
            var current = EndScope(ScopeType.Array);
            if (current.ObjectCount != 0)
            {
                Writer.WriteLine();
                DecreaseIndentation();
                WriteIndentation();
            }
            else
            {
                Writer.Write(WriterConstants.WhiteSpaceForEmptyArray);
                DecreaseIndentation();
            }

            Writer.Write(WriterConstants.EndArrayScope);
        }

        /// <summary>
        /// Write JSON end array.
        /// </summary>
        public override async Task WriteEndArrayAsync()
        {
            var current = EndScope(ScopeType.Array);
            if (current.ObjectCount != 0)
            {
                await Writer.WriteLineAsync();
                DecreaseIndentation();
                await WriteIndentationAsync();
            }
            else
            {
                await Writer.WriteAsync(WriterConstants.WhiteSpaceForEmptyArray);
                DecreaseIndentation();
            }

            await Writer.WriteAsync(WriterConstants.EndArrayScope);
        }

        /// <summary>
        /// Write property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// public override void WritePropertyName(string name)
        public override void WritePropertyName(string name)
        {
            VerifyCanWritePropertyName(name);

            var currentScope = CurrentScope();
            if (currentScope.ObjectCount != 0)
            {
                Writer.Write(WriterConstants.ObjectMemberSeparator);
            }

            Writer.WriteLine();

            currentScope.ObjectCount++;

            WriteIndentation();

            name = name.GetJsonCompatibleString();

            Writer.Write(name);

            Writer.Write(WriterConstants.NameValueSeparator);
        }

        /// <summary>
        /// Write property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// public override void WritePropertyName(string name)
        public override async Task WritePropertyNameAsync(string name)
        {
            VerifyCanWritePropertyName(name);

            var currentScope = CurrentScope();
            if (currentScope.ObjectCount != 0)
            {
                await Writer.WriteAsync(WriterConstants.ObjectMemberSeparator);
            }

            await Writer.WriteLineAsync();

            currentScope.ObjectCount++;

            await WriteIndentationAsync();

            name = name.GetJsonCompatibleString();

            await Writer.WriteAsync(name);

            await Writer.WriteAsync(WriterConstants.NameValueSeparator);
        }

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public override void WriteValue(string value)
        {
            WriteValueSeparator();

            value = value.GetJsonCompatibleString();

            Writer.Write(value);
        }

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public override async Task WriteValueAsync(string value)
        {
            await WriteValueSeparatorAsync();

            value = value.GetJsonCompatibleString();

            await Writer.WriteAsync(value);
        }


        /// <summary>
        /// Write null value.
        /// </summary>
        public override void WriteNull()
        {
            WriteValueSeparator();

            Writer.Write("null");
        }

        /// <summary>
        /// Write null value.
        /// </summary>
        public override async Task WriteNullAsync()
        {
            await WriteValueSeparatorAsync();

            await Writer.WriteAsync("null");
        }

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        protected override void WriteValueSeparator()
        {
            if (Scopes.Count == 0)
            {
                return;
            }

            var currentScope = Scopes.Peek();

            if (currentScope.Type == ScopeType.Array)
            {
                if (currentScope.ObjectCount != 0)
                {
                    Writer.Write(WriterConstants.ArrayElementSeparator);
                }

                Writer.WriteLine();
                WriteIndentation();
                currentScope.ObjectCount++;
            }
        }

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        protected override async Task WriteValueSeparatorAsync()
        {
            if (Scopes.Count == 0)
            {
                return;
            }

            var currentScope = Scopes.Peek();

            if (currentScope.Type == ScopeType.Array)
            {
                if (currentScope.ObjectCount != 0)
                {
                    await Writer.WriteAsync(WriterConstants.ArrayElementSeparator);
                }

                await Writer.WriteLineAsync();
                await WriteIndentationAsync();
                currentScope.ObjectCount++;
            }
        }

        /// <summary>
        /// Writes the content raw value.
        /// </summary>
        public override void WriteRaw(string value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Writes the content raw value.
        /// </summary>
        public override async Task WriteRawAsync(string value)
        {
            await WriteValueSeparatorAsync();
            await Writer.WriteAsync(value);
        }
    }
}