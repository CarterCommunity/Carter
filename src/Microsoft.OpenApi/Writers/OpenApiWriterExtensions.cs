// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Interfaces;

namespace Microsoft.OpenApi.Writers
{
    /// <summary>
    /// Extension methods for writing Open API documentation.
    /// </summary>
    public static class OpenApiWriterExtensions
    {
        /// <summary>
        /// Write a string property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteProperty(this IOpenApiWriter writer, string name, string value)
        {
            if (value == null)
            {
                return;
            }

            CheckArguments(writer, name);
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        /// <summary>
        /// Write a string property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static async Task WritePropertyAsync(this IOpenApiWriter writer, string name, string value)
        {
            if (value == null)
            {
                return;
            }

            CheckArguments(writer, name);
            await writer.WritePropertyNameAsync(name);
            await writer.WriteValueAsync(value);
        }

        /// <summary>
        /// Write a boolean property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default boolean value.</param>
        public static void WriteProperty(this IOpenApiWriter writer, string name, bool value, bool defaultValue = false)
        {
            if (value == defaultValue)
            {
                return;
            }

            CheckArguments(writer, name);
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        /// <summary>
        /// Write a boolean property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default boolean value.</param>
        public static async Task WritePropertyAsync(this IOpenApiWriter writer, string name, bool value, bool defaultValue = false)
        {
            if (value == defaultValue)
            {
                return;
            }

            CheckArguments(writer, name);
            await writer.WritePropertyNameAsync(name);
            await writer.WriteValueAsync(value);
        }

        /// <summary>
        /// Write a boolean property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default boolean value.</param>
        public static void WriteProperty(
            this IOpenApiWriter writer,
            string name,
            bool? value,
            bool defaultValue = false)
        {
            if (value == null || value.Value == defaultValue)
            {
                return;
            }

            CheckArguments(writer, name);
            writer.WritePropertyName(name);
            writer.WriteValue(value.Value);
        }

        /// <summary>
        /// Write a boolean property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="defaultValue">The default boolean value.</param>
        public static async Task WritePropertyAsync(
            this IOpenApiWriter writer,
            string name,
            bool? value,
            bool defaultValue = false)
        {
            if (value == null || value.Value == defaultValue)
            {
                return;
            }

            CheckArguments(writer, name);
            await writer.WritePropertyNameAsync(name);
            await writer.WriteValueAsync(value.Value);
        }

        /// <summary>
        /// Write a primitive property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteProperty<T>(this IOpenApiWriter writer, string name, T? value)
            where T : struct
        {
            if (value == null)
            {
                return;
            }

            writer.WriteProperty(name, value.Value);
        }

        /// <summary>
        /// Write a primitive property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static async Task WritePropertyAsync<T>(this IOpenApiWriter writer, string name, T? value)
            where T : struct
        {
            if (value == null)
            {
                return;
            }

            await writer.WritePropertyAsync(name, value.Value);
        }

        /// <summary>
        /// Write a string/number property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static void WriteProperty<T>(this IOpenApiWriter writer, string name, T value)
            where T : struct
        {
            CheckArguments(writer, name);
            writer.WritePropertyName(name);
            writer.WriteValue(value);
        }

        /// <summary>
        /// Write a string/number property.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        public static async Task WritePropertyAsync<T>(this IOpenApiWriter writer, string name, T value)
            where T : struct
        {
            CheckArguments(writer, name);
            await writer.WritePropertyNameAsync(name);
            await writer.WriteValueAsync(value);
        }

        /// <summary>
        /// Write the optional Open API object/element.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="action">The proprety value writer action.</param>
        public static void WriteOptionalObject<T>(
            this IOpenApiWriter writer,
            string name,
            T value,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            if (value != null)
            {
                var values = value as IEnumerable;
                if (values != null && !values.GetEnumerator().MoveNext())
                {
                    return; // Don't render optional empty collections
                }

                writer.WriteRequiredObject(name, value, action);
            }
        }

        /// <summary>
        /// Write the optional Open API object/element.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="action">The proprety value writer action.</param>
        public static async Task WriteOptionalObjectAsync<T>(
            this IOpenApiWriter writer,
            string name,
            T value,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            if (value != null)
            {
                var values = value as IEnumerable;
                if (values != null && !values.GetEnumerator().MoveNext())
                {
                    return; // Don't render optional empty collections
                }

                await writer.WriteRequiredObjectAsync(name, value, action);
            }
        }

        /// <summary>
        /// Write the required Open API object/element.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="action">The proprety value writer action.</param>
        public static void WriteRequiredObject<T>(
            this IOpenApiWriter writer,
            string name,
            T value,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            CheckArguments(writer, name, action);

            writer.WritePropertyName(name);
            if (value != null)
            {
                action(writer, value);
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Write the required Open API object/element.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="action">The proprety value writer action.</param>
        public static async Task WriteRequiredObjectAsync<T>(
            this IOpenApiWriter writer,
            string name,
            T value,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            CheckArguments(writer, name, action);

            await writer.WritePropertyNameAsync(name);
            if (value != null)
            {
                action(writer, value);
            }
            else
            {
                await writer.WriteStartObjectAsync();
                await writer.WriteEndObjectAsync();
            }
        }

        /// <summary>
        /// Write the optional of collection string.
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection string writer action.</param>
        public static void WriteOptionalCollection(
            this IOpenApiWriter writer,
            string name,
            IEnumerable<string> elements,
            Action<IOpenApiWriter, string> action)
        {
            if (elements != null && elements.Any())
            {
                writer.WriteCollectionInternal(name, elements, action);
            }
        }

        /// <summary>
        /// Write the optional of collection string.
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection string writer action.</param>
        public static async Task WriteOptionalCollectionAsync(
            this IOpenApiWriter writer,
            string name,
            IEnumerable<string> elements,
            Action<IOpenApiWriter, string> action)
        {
            if (elements != null && elements.Any())
            {
                await writer.WriteCollectionInternalAsync(name, elements, action);
            }
        }

        /// <summary>
        /// Write the optional Open API object/element collection.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection element writer action.</param>
        public static void WriteOptionalCollection<T>(
            this IOpenApiWriter writer,
            string name,
            IEnumerable<T> elements,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            if (elements != null && elements.Any())
            {
                writer.WriteCollectionInternal(name, elements, action);
            }
        }

        /// <summary>
        /// Write the optional Open API object/element collection.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection element writer action.</param>
        public static async Task WriteOptionalCollectionAsync<T>(
            this IOpenApiWriter writer,
            string name,
            IEnumerable<T> elements,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            if (elements != null && elements.Any())
            {
                await writer.WriteCollectionInternalAsync(name, elements, action);
            }
        }

        /// <summary>
        /// Write the required Open API object/element collection.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection element writer action.</param>
        public static void WriteRequiredCollection<T>(
            this IOpenApiWriter writer,
            string name,
            IEnumerable<T> elements,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            writer.WriteCollectionInternal(name, elements, action);
        }

        /// <summary>
        /// Write the required Open API object/element collection.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The collection values.</param>
        /// <param name="action">The collection element writer action.</param>
        public static async Task WriteRequiredCollectionAsync<T>(
            this IOpenApiWriter writer,
            string name,
            IEnumerable<T> elements,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            await writer.WriteCollectionInternalAsync(name, elements, action);
        }

        /// <summary>
        /// Write the optional Open API element map (string to string mapping).
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static void WriteOptionalMap(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, string> elements,
            Action<IOpenApiWriter, string> action)
        {
            if (elements != null && elements.Any())
            {
                writer.WriteMapInternal(name, elements, action);
            }
        }


        /// <summary>
        /// Write the optional Open API element map (string to string mapping).
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static async Task WriteOptionalMapAsync(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, string> elements,
            Action<IOpenApiWriter, string> action)
        {
            if (elements != null && elements.Any())
            {
                await writer.WriteMapInternalAsync(name, elements, action);
            }
        }

        /// <summary>
        /// Write the required Open API element map (string to string mapping).
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static void WriteRequiredMap(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, string> elements,
            Action<IOpenApiWriter, string> action)
        {
            writer.WriteMapInternal(name, elements, action);
        }

        /// <summary>
        /// Write the required Open API element map (string to string mapping).
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static async Task WriteRequiredMapAsync(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, string> elements,
            Action<IOpenApiWriter, string> action)
        {
            await writer.WriteMapInternalAsync(name, elements, action);
        }

        /// <summary>
        /// Write the optional Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action with writer and value as input.</param>
        public static void WriteOptionalMap<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            if (elements != null && elements.Any())
            {
                writer.WriteMapInternal(name, elements, action);
            }
        }
        
        /// <summary>
        /// Write the optional Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action with writer and value as input.</param>
        public static async Task WriteOptionalMapAsync<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            if (elements != null && elements.Any())
            {
                await writer.WriteMapInternalAsync(name, elements, action);
            }
        }

        /// <summary>
        /// Write the optional Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action with writer, key, and value as input.</param>
        public static void WriteOptionalMap<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, string, T> action)
            where T : IOpenApiElement
        {
            if (elements != null && elements.Any())
            {
                writer.WriteMapInternal(name, elements, action);
            }
        }
        
        /// <summary>
        /// Write the optional Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action with writer, key, and value as input.</param>
        public static async Task WriteOptionalMapAsync<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, string, T> action)
            where T : IOpenApiElement
        {
            if (elements != null && elements.Any())
            {
                await writer.WriteMapInternalAsync(name, elements, action);
            }
        }
        
        /// <summary>
        /// Write the required Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static void WriteRequiredMap<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            writer.WriteMapInternal(name, elements, action);
        }
        
        /// <summary>
        /// Write the required Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="action">The map element writer action.</param>
        public static async Task WriteRequiredMapAsync<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, T> action)
            where T : IOpenApiElement
        {
            await writer.WriteMapInternalAsync(name, elements, action);
        }

        private static void WriteCollectionInternal<T>(
            this IOpenApiWriter writer,
            string name,
            IEnumerable<T> elements,
            Action<IOpenApiWriter, T> action)
        {
            CheckArguments(writer, name, action);

            writer.WritePropertyName(name);
            writer.WriteStartArray();
            if (elements != null)
            {
                foreach (var item in elements)
                {
                    if (item != null)
                    {
                        action(writer, item);
                    }
                    else
                    {
                        writer.WriteNull();
                    }
                }
            }

            writer.WriteEndArray();
        }
        
        private static async Task WriteCollectionInternalAsync<T>(
            this IOpenApiWriter writer,
            string name,
            IEnumerable<T> elements,
            Action<IOpenApiWriter, T> action)
        {
            CheckArguments(writer, name, action);

            await writer.WritePropertyNameAsync(name);
            await writer.WriteStartArrayAsync();
            if (elements != null)
            {
                foreach (var item in elements)
                {
                    if (item != null)
                    {
                        action(writer, item);
                    }
                    else
                    {
                        await writer.WriteNullAsync();
                    }
                }
            }

            await writer.WriteEndArrayAsync();
        }

        private static void WriteMapInternal<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, T> action)
        {
            WriteMapInternal(writer, name, elements, (w, k, s) => action(w, s));
        }
        
        private static async Task WriteMapInternalAsync<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, T> action)
        {
            await WriteMapInternalAsync(writer, name, elements, (w, k, s) => action(w, s));
        }

        private static void WriteMapInternal<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, string, T> action)
        {
            CheckArguments(writer, name, action);

            writer.WritePropertyName(name);
            writer.WriteStartObject();

            if (elements != null)
            {
                foreach (var item in elements)
                {
                    writer.WritePropertyName(item.Key);
                    if (item.Value != null)
                    {
                        action(writer, item.Key, item.Value);
                    }
                    else
                    {
                        writer.WriteNull();
                    }
                }
            }

            writer.WriteEndObject();
        }
        
        private static async Task WriteMapInternalAsync<T>(
            this IOpenApiWriter writer,
            string name,
            IDictionary<string, T> elements,
            Action<IOpenApiWriter, string, T> action)
        {
            CheckArguments(writer, name, action);

            await writer.WritePropertyNameAsync(name);
            await writer.WriteStartObjectAsync();

            if (elements != null)
            {
                foreach (var item in elements)
                {
                    await writer.WritePropertyNameAsync(item.Key);
                    if (item.Value != null)
                    {
                        action(writer, item.Key, item.Value);
                    }
                    else
                    {
                        await writer.WriteNullAsync();
                    }
                }
            }

            await writer.WriteEndObjectAsync();
        }

        private static void CheckArguments<T>(IOpenApiWriter writer, string name, Action<IOpenApiWriter, T> action)
        {
            CheckArguments(writer, name);

            if (action == null)
            {
                throw Error.ArgumentNull(nameof(action));
            }
        }

        private static void CheckArguments<T>(IOpenApiWriter writer, string name, Action<IOpenApiWriter, string, T> action)
        {
            CheckArguments(writer, name);

            if (action == null)
            {
                throw Error.ArgumentNull(nameof(action));
            }
        }

        private static void CheckArguments(IOpenApiWriter writer, string name)
        {
            if (writer == null)
            {
                throw Error.ArgumentNull(nameof(writer));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw Error.ArgumentNullOrWhiteSpace(nameof(name));
            }
        }
    }
}