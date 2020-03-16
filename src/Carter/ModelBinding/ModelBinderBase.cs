namespace Carter.ModelBinding
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public abstract class ModelBinderBase : IModelBinder
    {
        private readonly List<Type> exceptionTypesToHandleWithDefault;

        protected ModelBinderBase(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(GetType());
            this.exceptionTypesToHandleWithDefault = new List<Type>();
        }

        protected ILogger Logger { get; }

        /// <summary>
        /// Registers exception types that should be ignored, causing the binder 
        /// to fallback to returning the default value of T
        /// </summary>
        /// <typeparam name="TException">The exception type to catch</typeparam>
        protected void HandleExceptionWithDefaultValue<TException>() where TException : Exception
        {
            this.exceptionTypesToHandleWithDefault.Add(typeof(TException));
        }

        public async Task<T> Bind<T>(HttpRequest request)
        {
            Logger.LogDebug("Binding [{Method}] {Path} to {Type}", request.Method,
                request.Path, typeof(T).FullName);

            try
            {
                var result = await BindCore<T>(request).ConfigureAwait(false);

                Logger.LogDebug("Binding OK for [{Method}] {Path} to {Type}", request.Method,
                    request.Path, typeof(T).FullName);

                return result;
            }
            catch (Exception ex) when (exceptionTypesToHandleWithDefault.Contains(ex.GetType()))
            {
                var result = typeof(T).IsValueType == false && typeof(T) != typeof(string)
                    ? Activator.CreateInstance<T>() 
                    : default;

                Logger.LogDebug("Handled Binding Exception for [{Method}] {Path} to {Type}. Returning default value.", request.Method,
                    request.Path, typeof(T).FullName);

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unhandled Binding Exception for [{Method}] {Path} to {Type}", request.Method,
                    request.Path, typeof(T).FullName);

                throw;
            }
        }

        protected abstract Task<T> BindCore<T>(HttpRequest request);
    }
}
