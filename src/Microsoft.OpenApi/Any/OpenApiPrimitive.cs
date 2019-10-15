// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Properties;
using Microsoft.OpenApi.Writers;

namespace Microsoft.OpenApi.Any
{
    /// <summary>
    /// Open API primitive class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OpenApiPrimitive<T> : IOpenApiPrimitive
    {
        /// <summary>
        /// Initializes the <see cref="IOpenApiPrimitive"/> class with the given value.
        /// </summary>
        /// <param name="value"></param>
        public OpenApiPrimitive(T value)
        {
            Value = value;
        }

        /// <summary>
        /// The kind of <see cref="IOpenApiAny"/>.
        /// </summary>
        public AnyType AnyType { get; } = AnyType.Primitive;

        /// <summary>
        /// The primitive class this object represents.
        /// </summary>
        public abstract PrimitiveType PrimitiveType { get; }

        /// <summary>
        /// Value of this <see cref="IOpenApiPrimitive"/>
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Write out content of primitive element
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="specVersion"></param>
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            switch (this.PrimitiveType)
            {
                case PrimitiveType.Integer:
                    var intValue = (OpenApiInteger)(IOpenApiPrimitive)this;
                    writer.WriteValue(intValue.Value);
                    break;

                case PrimitiveType.Long:
                    var longValue = (OpenApiLong)(IOpenApiPrimitive)this;
                    writer.WriteValue(longValue.Value);
                    break;

                case PrimitiveType.Float:
                    var floatValue = (OpenApiFloat)(IOpenApiPrimitive)this;
                    writer.WriteValue(floatValue.Value);
                    break;

                case PrimitiveType.Double:
                    var doubleValue = (OpenApiDouble)(IOpenApiPrimitive)this;
                    writer.WriteValue(doubleValue.Value);
                    break;

                case PrimitiveType.String:
                    var stringValue = (OpenApiString)(IOpenApiPrimitive)this;
                    writer.WriteValue(stringValue.Value);
                    break;

                case PrimitiveType.Byte:
                    var byteValue = (OpenApiByte)(IOpenApiPrimitive)this;
                    if (byteValue.Value == null)
                    {
                        writer.WriteNull();
                    }
                    else
                    {
                        writer.WriteValue(Convert.ToBase64String(byteValue.Value));
                    }

                    break;

                case PrimitiveType.Binary:
                    var binaryValue = (OpenApiBinary)(IOpenApiPrimitive)this;
                    if (binaryValue.Value == null)
                    {
                        writer.WriteNull();
                    }
                    else
                    {
                        writer.WriteValue(Encoding.UTF8.GetString(binaryValue.Value));
                    }

                    break;

                case PrimitiveType.Boolean:
                    var boolValue = (OpenApiBoolean)(IOpenApiPrimitive)this;
                    writer.WriteValue(boolValue.Value);
                    break;

                case PrimitiveType.Date:
                    var dateValue = (OpenApiDate)(IOpenApiPrimitive)this;
                    writer.WriteValue(dateValue.Value);
                    break;

                case PrimitiveType.DateTime:
                    var dateTimeValue = (OpenApiDateTime)(IOpenApiPrimitive)this;
                    writer.WriteValue(dateTimeValue.Value);
                    break;

                case PrimitiveType.Password:
                    var passwordValue = (OpenApiPassword)(IOpenApiPrimitive)this;
                    writer.WriteValue(passwordValue.Value);
                    break;

                default:
                    throw new OpenApiWriterException(
                        string.Format(
                            SRResource.PrimitiveTypeNotSupported,
                            this.PrimitiveType));
            }

        }
        
        /// <summary>
        /// Write out content of primitive element
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="specVersion"></param>
        public async Task WriteAsync(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            switch (this.PrimitiveType)
            {
                case PrimitiveType.Integer:
                    var intValue = (OpenApiInteger)(IOpenApiPrimitive)this;
                    await writer.WriteValueAsync(intValue.Value);
                    break;

                case PrimitiveType.Long:
                    var longValue = (OpenApiLong)(IOpenApiPrimitive)this;
                    await writer.WriteValueAsync(longValue.Value);
                    break;

                case PrimitiveType.Float:
                    var floatValue = (OpenApiFloat)(IOpenApiPrimitive)this;
                    await writer.WriteValueAsync(floatValue.Value);
                    break;

                case PrimitiveType.Double:
                    var doubleValue = (OpenApiDouble)(IOpenApiPrimitive)this;
                    await writer.WriteValueAsync(doubleValue.Value);
                    break;

                case PrimitiveType.String:
                    var stringValue = (OpenApiString)(IOpenApiPrimitive)this;
                    await writer.WriteValueAsync(stringValue.Value);
                    break;

                case PrimitiveType.Byte:
                    var byteValue = (OpenApiByte)(IOpenApiPrimitive)this;
                    if (byteValue.Value == null)
                    {
                        await writer.WriteNullAsync();
                    }
                    else
                    {
                        await writer.WriteValueAsync(Convert.ToBase64String(byteValue.Value));
                    }

                    break;

                case PrimitiveType.Binary:
                    var binaryValue = (OpenApiBinary)(IOpenApiPrimitive)this;
                    if (binaryValue.Value == null)
                    {
                        await writer.WriteNullAsync();
                    }
                    else
                    {
                        await writer.WriteValueAsync(Encoding.UTF8.GetString(binaryValue.Value));
                    }

                    break;

                case PrimitiveType.Boolean:
                    var boolValue = (OpenApiBoolean)(IOpenApiPrimitive)this;
                    await writer.WriteValueAsync(boolValue.Value);
                    break;

                case PrimitiveType.Date:
                    var dateValue = (OpenApiDate)(IOpenApiPrimitive)this;
                    await writer.WriteValueAsync(dateValue.Value);
                    break;

                case PrimitiveType.DateTime:
                    var dateTimeValue = (OpenApiDateTime)(IOpenApiPrimitive)this;
                    await writer.WriteValueAsync(dateTimeValue.Value);
                    break;

                case PrimitiveType.Password:
                    var passwordValue = (OpenApiPassword)(IOpenApiPrimitive)this;
                    await writer.WriteValueAsync(passwordValue.Value);
                    break;

                default:
                    throw new OpenApiWriterException(
                        string.Format(
                            SRResource.PrimitiveTypeNotSupported,
                            this.PrimitiveType));
            }

        }
    }
}