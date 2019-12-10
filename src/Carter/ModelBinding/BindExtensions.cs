namespace Carter.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Carter.Request;
    using FluentValidation.Results;
    using Microsoft.AspNetCore.Http;

    public static class BindExtensions
    {
        /// <summary>
        /// Bind the incoming request body to a model and validate it
        /// </summary>
        /// <param name="request">Current <see cref="HttpRequest"/></param>
        /// <typeparam name="T">Model type</typeparam>
        /// <returns><see cref="ValidationResult"/> and bound model</returns>
        public static async Task<(ValidationResult ValidationResult, T Data)> BindAndValidate<T>(this HttpRequest request)
        {
            var model = await request.Bind<T>();

            var validationResult = request.Validate(model);
            return (validationResult, model);
        }

        /// <summary>
        /// Bind the incoming request body to a model
        /// </summary>
        /// <param name="request">Current <see cref="HttpRequest"/></param>
        /// <typeparam name="T">Model type</typeparam>
        /// <returns>Bound model</returns>
        public static async Task<T> Bind<T>(this HttpRequest request)
        {
            if (request.HasFormContentType)
            {
                var res = request.Form.ToDictionary(key => key.Key, val =>
                {
                    var type = typeof(T);
                    var propertyType = type.GetProperty(val.Key)?.PropertyType;

                    if (propertyType == null)
                    {
                        return null;
                    }

                    if (propertyType.IsArray() || propertyType.IsCollection() || propertyType.IsEnumerable())
                    {
                        var colType = propertyType.GetElementType();
                        if (colType == null)
                        {
                            colType = propertyType.GetGenericArguments().First();
                        }

                        return val.Value.Select(y => { return ConvertToType(y, colType); });
                    }

                    return ConvertToType(val.Value[0], propertyType);
                });

                var json = JsonSerializer.Serialize(res);
                return JsonSerializer.Deserialize<T>(json);
            }

            try
            {
                return await JsonSerializer.DeserializeAsync<T>(request.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                return typeof(T).IsValueType == false ? Activator.CreateInstance<T>() : default;
            }
        }

        private static object ConvertToType(string value, Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);

            if (value.Length > 0)
            {
                if (type == typeof(DateTime) || underlyingType == typeof(DateTime))
                {
                    return DateTime.Parse(value, CultureInfo.InvariantCulture);
                }

                if (type == typeof(Guid) || underlyingType == typeof(Guid))
                {
                    return new Guid(value);
                }

                if (type == typeof(Uri) || underlyingType == typeof(Uri))
                {
                    if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out Uri uri))
                    {
                        return uri;
                    }

                    return null;
                }
            }
            else
            {
                if (type == typeof(Guid))
                {
                    return default(Guid);
                }

                if (underlyingType != null)
                {
                    return null;
                }
            }

            if (underlyingType is object)
            {
                return Convert.ChangeType(value, underlyingType);
            }

            return Convert.ChangeType(value, type);
        }

        private static async Task<IEnumerable<IFormFile>> BindFiles(this HttpRequest request, bool returnOnFirst)
        {
            var postedFiles = new List<IFormFile>();

            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync();

                foreach (var file in form.Files)
                {
                    // If there is an <input type="file" ... /> in the form and is left blank.
                    if (file.Length == 0 && string.IsNullOrEmpty(file.FileName))
                    {
                        continue;
                    }

                    postedFiles.Add(file);

                    if (returnOnFirst)
                    {
                        return postedFiles;
                    }
                }
            }

            return postedFiles;
        }

        /// <summary>
        /// Bind the <see cref="HttpRequest"/> form and return an <see cref="IEnumerable{IFormFile}"/> of files
        /// </summary>
        /// <param name="request">Current <see cref="HttpRequest"/></param>
        /// <returns><see cref="IEnumerable{IFormFile}"/></returns>
        public static Task<IEnumerable<IFormFile>> BindFiles(this HttpRequest request)
        {
            return request.BindFiles(returnOnFirst: false);
        }

        /// <summary>
        /// Bind the <see cref="HttpRequest"/> form and return the first <see cref="IFormFile"/>
        /// </summary>
        /// <param name="request">Current <see cref="HttpRequest"/></param>
        /// <returns><see cref="IFormFile"/></returns>
        public static async Task<IFormFile> BindFile(this HttpRequest request)
        {
            var files = await request.BindFiles(returnOnFirst: true);

            return files.First();
        }

        /// <summary>
        /// Save all files in the <see cref="HttpRequest"/> form to the path provided by <param name="saveLocation"></param>
        /// </summary>
        /// <param name="request">Current <see cref="HttpRequest"/></param>
        /// <param name="saveLocation">The location of where to save the file</param>
        /// <returns>Awaited <see cref="Task"/></returns>
        public static async Task BindAndSaveFiles(this HttpRequest request, string saveLocation)
        {
            var files = await request.BindFiles();

            foreach (var file in files)
                await SaveFileInternal(file, saveLocation);
        }

        /// <summary>
        /// Save the first file in the <see cref="HttpRequest"/> form to the path provided by <param name="fileName"></param>
        /// </summary>
        /// <param name="request">Current <see cref="HttpRequest"/></param>
        /// <param name="saveLocation">The location of where to save the file</param>
        /// <param name="fileName">The filename to use when saving the file</param>
        /// <returns>Awaited <see cref="Task"/></returns>
        public static async Task BindAndSaveFile(this HttpRequest request, string saveLocation, string fileName = "")
        {
            var file = await request.BindFile();

            await SaveFileInternal(file, saveLocation, fileName);
        }

        private static async Task SaveFileInternal(IFormFile file, string saveLocation, string fileName = "")
        {
            if (!Directory.Exists(saveLocation))
                Directory.CreateDirectory(saveLocation);

            fileName = !string.IsNullOrWhiteSpace(fileName) ? fileName : file.FileName;

            using (var fileToSave = File.Create(Path.Combine(saveLocation, fileName)))
                await file.CopyToAsync(fileToSave);
        }
    }
}
