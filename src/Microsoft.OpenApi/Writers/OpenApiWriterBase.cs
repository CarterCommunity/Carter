// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Base class for Open API writer.
    /// </summary>
    public abstract class OpenApiWriterBase : IOpenApiWriter
    {
        /// <summary>
        /// The indentation string to prepand to each line for each indentation level.
        /// </summary>
        private const string IndentationString = "  ";

        /// <summary>
        /// Scope of the Open API element - object, array, property.
        /// </summary>
        protected readonly Stack<Scope> Scopes;

        /// <summary>
        /// Number which specifies the level of indentation.
        /// </summary>
        private int _indentLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiWriterBase"/> class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        public OpenApiWriterBase(TextWriter textWriter)
        {
            Writer = textWriter;
            Writer.NewLine = "\n";

            Scopes = new Stack<Scope>();
        }

        /// <summary>
        /// Base Indentation Level.
        /// This denotes how many indentations are needed for the property in the base object.
        /// </summary>
        protected abstract int BaseIndentation { get; }

        /// <summary>
        /// The text writer.
        /// </summary>
        protected TextWriter Writer { get; }

        /// <summary>
        /// Write start object.
        /// </summary>
        public abstract void WriteStartObject();

        /// <summary>
        /// Write start object.
        /// </summary>
        public abstract Task WriteStartObjectAsync();

        /// <summary>
        /// Write end object.
        /// </summary>
        public abstract void WriteEndObject();

        /// <summary>
        /// Write end object.
        /// </summary>
        public abstract Task WriteEndObjectAsync();

        /// <summary>
        /// Write start array.
        /// </summary>
        public abstract void WriteStartArray();

        /// <summary>
        /// Write start array.
        /// </summary>
        public abstract Task WriteStartArrayAsync();

        /// <summary>
        /// Write end array.
        /// </summary>
        public abstract void WriteEndArray();

        /// <summary>
        /// Write end array.
        /// </summary>
        public abstract Task WriteEndArrayAsync();

        /// <summary>
        /// Write the start property.
        /// </summary>
        public abstract void WritePropertyName(string name);

        /// <summary>
        /// Write the start property.
        /// </summary>
        public abstract Task WritePropertyNameAsync(string name);

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        protected abstract void WriteValueSeparator();

        /// <summary>
        /// Writes a separator of a value if it's needed for the next value to be written.
        /// </summary>
        protected abstract Task WriteValueSeparatorAsync();

        /// <summary>
        /// Write null value.
        /// </summary>
        public abstract void WriteNull();

        /// <summary>
        /// Write null value.
        /// </summary>
        public abstract Task WriteNullAsync();

        /// <summary>
        /// Write content raw value.
        /// </summary>
        public abstract void WriteRaw(string value);

        /// <summary>
        /// Write content raw value.
        /// </summary>
        public abstract Task WriteRawAsync(string value);

        /// <summary>
        /// Flush the writer.
        /// </summary>
        public void Flush()
        {
            Writer.Flush();
        }

        /// <summary>
        /// Flush the writer.
        /// </summary>
        public async Task FlushAsync()
        {
            await Writer.FlushAsync();
        }
        
        /// <summary>
        /// Write decimal value.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        public virtual async Task WriteValueAsync(decimal value)
        {
            await WriteValueSeparatorAsync();
            await Writer.WriteAsync(value.ToString());
        }

        /// <summary>
        /// Write integer value.
        /// </summary>
        /// <param name="value">The integer value.</param>
        public virtual async Task WriteValueAsync(int value)
        {
            await WriteValueSeparatorAsync();
            await Writer.WriteAsync(value.ToString());
        }

        /// <summary>
        /// Write boolean value.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        public virtual async Task WriteValueAsync(bool value)
        {
            await WriteValueSeparatorAsync();
            await Writer.WriteAsync(value.ToString());
        }

        /// <summary>
        /// Write float value.
        /// </summary>
        /// <param name="value">The float value.</param>
        public virtual async Task WriteValueAsync(float value)
        {
            await WriteValueSeparatorAsync();
            await Writer.WriteAsync(value.ToString());
        }

        /// <summary>
        /// Write long value.
        /// </summary>
        /// <param name="value">The long value.</param>
        public virtual async Task WriteValueAsync(long value)
        {
            await WriteValueSeparatorAsync();
            await Writer.WriteAsync(value.ToString());
        }
        
        /// <summary>
        /// Write double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        public virtual async Task WriteValueAsync(double value)
        {
            await WriteValueSeparatorAsync();
            await Writer.WriteAsync(value.ToString());
        }
        
        /// <summary>
        /// Write DateTime value.
        /// </summary>
        /// <param name="value">The DateTime value.</param>
        public virtual async Task WriteValueAsync(DateTime value)
        {
            await this.WriteValueAsync(value.ToString("o"));
        }
        
        /// <summary>
        /// Write DateTimeOffset value.
        /// </summary>
        /// <param name="value">The DateTimeOffset value.</param>
        public virtual async Task WriteValueAsync(DateTimeOffset value)
        {
            await this.WriteValueAsync(value.ToString("o"));
        }

        /// <summary>
        /// Write object value.
        /// </summary>
        /// <param name="value">The object value.</param>
        public virtual async Task WriteValueAsync(object value)
        {
            if (value == null)
            {
                await WriteNullAsync();
                return;
            }

            var type = value.GetType();

            if (type == typeof(string))
            {
                await WriteValueAsync((string) (value));
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                await WriteValueAsync((int) value);
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                await WriteValueAsync((long) value);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                await WriteValueAsync((bool) value);
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                await WriteValueAsync((float) value);
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                await WriteValueAsync((double) value);
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                await WriteValueAsync((decimal) value);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                await WriteValueAsync((DateTime) value);
            }
            else if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                await WriteValueAsync((DateTimeOffset) value);
            }
            else
            {
                throw new OpenApiWriterException(string.Format(SRResource.OpenApiUnsupportedValueType, type.FullName));
            }
        }


        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public abstract void WriteValue(string value);

        /// <summary>
        /// Write string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public abstract Task WriteValueAsync(string value);


        /// <summary>
        /// Write float value.
        /// </summary>
        /// <param name="value">The float value.</param>
        public virtual void WriteValue(float value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Write double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        public virtual void WriteValue(double value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Write decimal value.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        public virtual void WriteValue(decimal value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Write integer value.
        /// </summary>
        /// <param name="value">The integer value.</param>
        public virtual void WriteValue(int value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Write long value.
        /// </summary>
        /// <param name="value">The long value.</param>
        public virtual void WriteValue(long value)
        {
            WriteValueSeparator();
            Writer.Write(value);
        }

        /// <summary>
        /// Write DateTime value.
        /// </summary>
        /// <param name="value">The DateTime value.</param>
        public virtual void WriteValue(DateTime value)
        {
            this.WriteValue(value.ToString("o"));
        }

        /// <summary>
        /// Write DateTimeOffset value.
        /// </summary>
        /// <param name="value">The DateTimeOffset value.</param>
        public virtual void WriteValue(DateTimeOffset value)
        {
            this.WriteValue(value.ToString("o"));
        }

        /// <summary>
        /// Write boolean value.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        public virtual void WriteValue(bool value)
        {
            WriteValueSeparator();
            Writer.Write(value.ToString().ToLower());
        }

        /// <summary>
        /// Write object value.
        /// </summary>
        /// <param name="value">The object value.</param>
        public virtual void WriteValue(object value)
        {
            if (value == null)
            {
                WriteNull();
                return;
            }

            var type = value.GetType();

            if (type == typeof(string))
            {
                WriteValue((string) (value));
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                WriteValue((int) value);
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                WriteValue((long) value);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                WriteValue((bool) value);
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                WriteValue((float) value);
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                WriteValue((double) value);
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                WriteValue((decimal) value);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                WriteValue((DateTime) value);
            }
            else if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                WriteValue((DateTimeOffset) value);
            }
            else
            {
                throw new OpenApiWriterException(string.Format(SRResource.OpenApiUnsupportedValueType, type.FullName));
            }
        }

        /// <summary>
        /// Increases the level of indentation applied to the output.
        /// </summary>
        public virtual void IncreaseIndentation()
        {
            _indentLevel++;
        }

        /// <summary>
        /// Decreases the level of indentation applied to the output.
        /// </summary>
        public virtual void DecreaseIndentation()
        {
            if (_indentLevel == 0)
            {
                throw new OpenApiWriterException(SRResource.IndentationLevelInvalid);
            }

            if (_indentLevel < 1)
            {
                _indentLevel = 0;
            }
            else
            {
                _indentLevel--;
            }
        }

        /// <summary>
        /// Write the indentation.
        /// </summary>
        public virtual void WriteIndentation()
        {
            for (var i = 0; i < (BaseIndentation + _indentLevel - 1); i++)
            {
                Writer.Write(IndentationString);
            }
        }

        /// <summary>
        /// Write the indentation.
        /// </summary>
        public virtual async Task WriteIndentationAsync()
        {
            for (var i = 0; i < (BaseIndentation + _indentLevel - 1); i++)
            {
                await Writer.WriteAsync(IndentationString);
            }
        }

        /// <summary>
        /// Get current scope.
        /// </summary>
        /// <returns></returns>
        protected Scope CurrentScope()
        {
            return Scopes.Count == 0 ? null : Scopes.Peek();
        }

        /// <summary>
        /// Start the scope given the scope type.
        /// </summary>
        /// <param name="type">The scope type to start.</param>
        protected Scope StartScope(ScopeType type)
        {
            if (Scopes.Count != 0)
            {
                var currentScope = Scopes.Peek();

                currentScope.ObjectCount++;
            }

            var scope = new Scope(type);
            Scopes.Push(scope);
            return scope;
        }

        /// <summary>
        /// End the scope of the given scope type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected Scope EndScope(ScopeType type)
        {
            if (Scopes.Count == 0)
            {
                throw new OpenApiWriterException(SRResource.ScopeMustBePresentToEnd);
            }

            if (Scopes.Peek().Type != type)
            {
                throw new OpenApiWriterException(
                    string.Format(
                        SRResource.ScopeToEndHasIncorrectType,
                        type,
                        Scopes.Peek().Type));
            }

            return Scopes.Pop();
        }

        /// <summary>
        /// Whether the current scope is the top level (outermost) scope.
        /// </summary>
        protected bool IsTopLevelScope()
        {
            return Scopes.Count == 1;
        }

        /// <summary>
        /// Whether the current scope is an object scope.
        /// </summary>
        protected bool IsObjectScope()
        {
            return IsScopeType(ScopeType.Object);
        }

        /// <summary>
        /// Whether the current scope is an array scope.
        /// </summary>
        /// <returns></returns>
        protected bool IsArrayScope()
        {
            return IsScopeType(ScopeType.Array);
        }

        private bool IsScopeType(ScopeType type)
        {
            if (Scopes.Count == 0)
            {
                return false;
            }

            return Scopes.Peek().Type == type;
        }

        /// <summary>
        /// Verifies whether a property name can be written based on whether
        /// the property name is a valid string and whether the current scope is an object scope.
        /// </summary>
        /// <param name="name">property name</param>
        protected void VerifyCanWritePropertyName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }

            if (Scopes.Count == 0)
            {
                throw new OpenApiWriterException(
                    string.Format(SRResource.ActiveScopeNeededForPropertyNameWriting, name));
            }

            if (Scopes.Peek().Type != ScopeType.Object)
            {
                throw new OpenApiWriterException(
                    string.Format(SRResource.ObjectScopeNeededForPropertyNameWriting, name));
            }
        }
    }
}