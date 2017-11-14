namespace Botwin
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentValidation;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    public static class BotwinExtensions
    {
        /// <summary>
        /// Adds Botwin to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder)
        {
            return UseBotwin(builder, null);
        }

        /// <summary>
        /// Adds Botwin to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to configure.</param>
        /// <param name="options">A <see cref="BotwinOptions"/> instance.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseBotwin(this IApplicationBuilder builder, BotwinOptions options)
        {
            ApplyGlobalBeforeHook(builder, options);

            ApplyGlobalAfterHook(builder, options);

            var routeBuilder = new RouteBuilder(builder);

            //Create a "startup scope" to resolve modules from
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                //Get all instances of BotwinModule to fetch and register declared routes
                foreach (var module in scope.ServiceProvider.GetServices<BotwinModule>())
                {
                    var moduleType = module.GetType();

                    foreach (var route in module.Routes)
                    {
                        routeBuilder.MapVerb(route.verb, route.path, ctx =>
                        {
                            var statusCodeHandlers = ctx.RequestServices.GetServices<IStatusCodeHandler>();

                            var requestScopedModule = ctx.RequestServices.GetRequiredService(moduleType) as BotwinModule;

                            // TODO: Use the handler from the resolved 'requestScopedModule' instead of the one from the "startup scope".
                            var handler = CreateModuleBeforeAfterHandler(requestScopedModule, route.handler);

                            var finalHandler = CreateFinalHandler(handler, statusCodeHandlers);

                            return finalHandler.Invoke(ctx);
                        });
                    }
                }
            }

            return builder.UseRouter(routeBuilder.Build());
        }

        private static RequestDelegate CreateFinalHandler(RequestDelegate handler, IEnumerable<IStatusCodeHandler> statusCodeHandlers)
        {
            RequestDelegate finalHandler = async (ctx) =>
            {
                if (HttpMethods.IsHead(ctx.Request.Method))
                {
                    //Cannot read the default stream once WriteAsync has been called on it
                    ctx.Response.Body = new MemoryStream();
                }

                await handler(ctx);

                var scHandler = statusCodeHandlers.FirstOrDefault(x => x.CanHandle(ctx.Response.StatusCode));

                if (scHandler != null)
                {
                    await scHandler.Handle(ctx);
                }

                if (HttpMethods.IsHead(ctx.Request.Method))
                {
                    var length = ctx.Response.Body.Length;
                    ctx.Response.Body.SetLength(0);
                    ctx.Response.ContentLength = length;
                }
            };
            return finalHandler;
        }

        private static RequestDelegate CreateModuleBeforeAfterHandler(BotwinModule module, RequestDelegate handler)
        {
            RequestDelegate afterHandler = async (context) =>
            {
                if (module.Before != null)
                {
                    var shouldContinue = await module.Before(context);
                    if (!shouldContinue)
                    {
                        return;
                    }
                }
                
                await handler(context);
                
                if (module.After != null)
                {
                    await module.After(context);
                }
            };

            return afterHandler;
        }

        private static void ApplyGlobalAfterHook(IApplicationBuilder builder, BotwinOptions options)
        {
            if (options?.After != null)
            {
                builder.Use(async (ctx, next) =>
                {
                    await next();
                    await options.After(ctx);
                });
            }
        }

        private static void ApplyGlobalBeforeHook(IApplicationBuilder builder, BotwinOptions options)
        {
            if (options?.Before != null)
            {
                builder.Use(async (ctx, next) =>
                {
                    var carryOn = await options.Before(ctx); //TODO Check if return Task.CompletedTask will it continue
                    if (carryOn)
                    {
                        await next();
                    }
                });
            }
        }

        /// <summary>
        /// Adds Botwin to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add Botwin to.</param>
        /// <param name="assemblies">Optional array of <see cref="Assembly"/> to add to the services collection. If assemblies are not provided, Assembly.GetEntryAssembly is called.</param>
        public static void AddBotwin(this IServiceCollection services, params Assembly[] assemblies)
        {
            assemblies = assemblies.Any() ? assemblies : new[] { Assembly.GetEntryAssembly() };

            var validators = assemblies.SelectMany(ass => ass.GetExportedTypes())
                .Where(typeof(IValidator).IsAssignableFrom)
                .Where(t => !t.GetTypeInfo().IsAbstract);

            foreach (var validator in validators)
            {
                services.AddSingleton(typeof(IValidator), validator);
            }

            services.AddSingleton<IValidatorLocator, DefaultValidatorLocator>();

            services.AddRouting();

            var modules = assemblies.SelectMany(x => x.GetTypes()
            .Where(
                t => !t.IsAbstract &&
                typeof(BotwinModule).IsAssignableFrom(t) && 
                t != typeof(BotwinModule)));
            foreach (var module in modules)
            {
                services.AddScoped(module);
                services.AddScoped(typeof(BotwinModule), module);
            }

            var schs = assemblies.SelectMany(x => x.GetTypes().Where(t => typeof(IStatusCodeHandler).IsAssignableFrom(t) && t != typeof(IStatusCodeHandler)));
            foreach (var sch in schs)
            {
                services.AddScoped(typeof(IStatusCodeHandler), sch);
            }

            var responseNegotiators = assemblies.SelectMany(x => x.GetTypes().Where(t => typeof(IResponseNegotiator).IsAssignableFrom(t) && t != typeof(IResponseNegotiator)));
            foreach (var negotiatator in responseNegotiators)
            {
                services.AddSingleton(typeof(IResponseNegotiator), negotiatator);
            }

            services.AddSingleton(typeof(IResponseNegotiator), new DefaultJsonResponseNegotiator());
        }
    }
}
