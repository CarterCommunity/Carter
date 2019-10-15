// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Extensions methods for writing the <see cref="IOpenApiAny"/>
    /// </summary>
    public static class OpenApiWriterAnyExtensions
    {
        /// <summary>
        /// Write the specification extensions
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="extensions">The specification extensions.</param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public static void WriteExtensions(this IOpenApiWriter writer, IDictionary<string, IOpenApiExtension> extensions, OpenApiSpecVersion specVersion)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (extensions != null)
            {
                foreach (var item in extensions)
                {
                    writer.WritePropertyName(item.Key);
                    item.Value.Write(writer, specVersion);
                }
            }
        }
        
        /// <summary>
        /// Write the specification extensions
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="extensions">The specification extensions.</param>
        /// <param name="specVersion">Version of the OpenAPI specification that that will be output.</param>
        public static async Task WriteExtensionsAsync(this IOpenApiWriter writer, IDictionary<string, IOpenApiExtension> extensions, OpenApiSpecVersion specVersion)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (extensions != null)
            {
                foreach (var item in extensions)
                {
                    await writer.WritePropertyNameAsync(item.Key);
                    item.Value.Write(writer, specVersion);
                }
            }
        }

        /// <summary>
        /// Write the <see cref="IOpenApiAny"/> value.
        /// </summary>
        /// <typeparam name="T">The Open API Any type.</typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="any">The Any value</param>
        public static void WriteAny<T>(this IOpenApiWriter writer, T any) where T : IOpenApiAny
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (any == null)
            {
                writer.WriteNull();
                return;
            }

            switch (any.AnyType)
            {
                case AnyType.Array: // Array
                    writer.WriteArray(any as OpenApiArray);
                    break;

                case AnyType.Object: // Object
                    writer.WriteObject(any as OpenApiObject);
                    break;

                case AnyType.Primitive: // Primitive
                    writer.WritePrimitive(any as IOpenApiPrimitive);
                    break;

                case AnyType.Null: // null
                    writer.WriteNull();
                    break;

                default:
                    break;
            }
        }
        
        /// <summary>
        /// Write the <see cref="IOpenApiAny"/> value.
        /// </summary>
        /// <typeparam name="T">The Open API Any type.</typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="any">The Any value</param>
        public static async Task WriteAnyAsync<T>(this IOpenApiWriter writer, T any) where T : IOpenApiAny
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (any == null)
            {
                await writer.WriteNullAsync();
                return;
            }

            switch (any.AnyType)
            {
                case AnyType.Array: // Array
                    await writer.WriteArrayAsync(any as OpenApiArray);
                    break;

                case AnyType.Object: // Object
                    await writer.WriteObjectAsync(any as OpenApiObject);
                    break;

                case AnyType.Primitive: // Primitive
                    await writer.WritePrimitiveAsync(any as IOpenApiPrimitive);
                    break;

                case AnyType.Null: // null
                    await writer.WriteNullAsync();
                    break;

                default:
                    break;
            }
        }

        private static void WriteArray(this IOpenApiWriter writer, OpenApiArray array)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (array == null)
            {
                throw Error.ArgumentNull(nameof(array));
            }

            writer.WriteStartArray();

            foreach (var item in array)
            {
                writer.WriteAny(item);
            }

            writer.WriteEndArray();
        }
        
        private static async Task WriteArrayAsync(this IOpenApiWriter writer, OpenApiArray array)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (array == null)
            {
                throw Error.ArgumentNull(nameof(array));
            }

            await writer.WriteStartArrayAsync();

            foreach (var item in array)
            {
               await writer.WriteAnyAsync(item);
            }

            await writer.WriteEndArrayAsync();
        }

        private static void WriteObject(this IOpenApiWriter writer, OpenApiObject entity)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (entity == null)
            {
                throw Error.ArgumentNull(nameof(entity));
            }

            writer.WriteStartObject();

            foreach (var item in entity)
            {
                writer.WritePropertyName(item.Key);
                writer.WriteAny(item.Value);
            }

            writer.WriteEndObject();
        }
        
        private static async Task WriteObjectAsync(this IOpenApiWriter writer, OpenApiObject entity)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (entity == null)
            {
                throw Error.ArgumentNull(nameof(entity));
            }

            await writer.WriteStartObjectAsync();

            foreach (var item in entity)
            {
                await writer.WritePropertyNameAsync(item.Key);
                await writer.WriteAnyAsync(item.Value);
            }

            await writer.WriteEndObjectAsync();
        }

        private static void WritePrimitive(this IOpenApiWriter writer, IOpenApiPrimitive primitive)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (primitive == null)
            {
                throw Error.ArgumentNull(nameof(primitive));
            }

            // The Spec version is meaning for the Any type, so it's ok to use the latest one.
            primitive.Write(writer, OpenApiSpecVersion.OpenApi3_0);
        }
        
        private static async Task WritePrimitiveAsync(this IOpenApiWriter writer, IOpenApiPrimitive primitive)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (primitive == null)
            {
                throw Error.ArgumentNull(nameof(primitive));
            }

            // The Spec version is meaning for the Any type, so it's ok to use the latest one.
            await primitive.WriteAsync(writer, OpenApiSpecVersion.OpenApi3_0);
        }
    }
}