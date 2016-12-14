using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SharpRaven;
using SharpRaven.AspNetCore;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up raven sharp related services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class RavenSharpServiceCollectionExtensions
    {
        /// <summary>
        /// Adds SharpRaven singleton <see cref="IRavenClient"/> to the
        /// <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddRavenClient(this IServiceCollection services)
        {
            AddRavenClient(services, ravenOptions => { });
            return services;
        }

        /// <summary>
        /// Adds sharp raven singleton of type <see cref="IRavenClient"/> to the
        /// <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">
        /// The <see cref="Action{RavenOptions}"/> to configure the provided <see cref="RavenClient"/>.
        /// </param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddRavenClient(this IServiceCollection services, Action<RavenOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            RavenOptions options = new RavenOptions();

            setupAction(options);

            services.AddOptions();
            services.Add(ServiceDescriptor.Singleton<IOptions<RavenOptions>>(new OptionsWrapper<RavenOptions>(options)));

            services.TryAdd(ServiceDescriptor.Singleton<IRavenClient, RavenClient>((serviceProvider) =>
            {
                var config = serviceProvider.GetService<IOptions<RavenOptions>>();
                var environment = serviceProvider.GetService<IHostingEnvironment>();
                string release = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;

                return new RavenClient(config.Value.Dsn)
                {
                    Compression = config.Value.Compression,
                    Environment = string.IsNullOrEmpty(environment.EnvironmentName) ? environment.EnvironmentName : config.Value.Environment,
                    IgnoreBreadcrumbs = config.Value.IgnoreBreadcrumbs,
                    Logger = config.Value.Logger,
                    Timeout = config.Value.Timeout,
                    Release = string.IsNullOrEmpty(config.Value.Release) ? release : config.Value.Release
                };
            }));

            services.TryAdd(ServiceDescriptor.Singleton<RavenExceptionFilter>(serviceProvider =>
            {
                var ravenClient = serviceProvider.GetService<IRavenClient>();
                return new RavenExceptionFilter(ravenClient);
            }));
            services.AddMvc(config =>
            {
                config.Filters.AddService(typeof(RavenExceptionFilter));
            });

            return services;
        }
    }
}
