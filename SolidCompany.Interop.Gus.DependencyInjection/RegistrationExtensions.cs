using System;
using Microsoft.Extensions.DependencyInjection;

namespace SolidCompany.Interop.Gus.DependencyInjection
{
    /// <summary>
    /// ServiceCollection registration extensions for <see cref="GusBirClient"/>).
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Adds <see cref="GusBirClient"/> as <see cref="IGusBirClient"/> to container and configures options.
        /// </summary>
        /// <param name="serviceCollection"><see cref="IServiceCollection"/></param>
        /// <param name="configuration">Configuratino options</param>
        /// <returns></returns>
        public static IServiceCollection AddGusClient(this IServiceCollection serviceCollection, Action<GusBirOptions> configuration)
        {
            serviceCollection.Configure(configuration);
            serviceCollection.AddScoped<IGusBirClient, GusBirClient>();

            return serviceCollection;
        }
    }
}
