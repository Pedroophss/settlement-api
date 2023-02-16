using Microsoft.Extensions.DependencyInjection;
using RetSettlementDates.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RetSettlementDates.Domain
{
    public static class DependenceInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            InjectSingletons<IDataService>(services);
            InjectTransients<IBinderService>(services);

            return services;
        }

        /* Production Notes:
         * This two methods above should be in a more generic place
         */

        public static void InjectSingletons<T>(IServiceCollection services)
        {
            var assembly = typeof(DependenceInjection).Assembly;
            var instances = assembly.DefinedTypes
                                    .Where(w => !w.IsInterface && !w.IsAbstract)
                                    .Where(w => w.GetInterface(typeof(T).Name) != null)
                                    .Select(s => (IDataService)Activator.CreateInstance(s))
                                    .ToList();

            foreach (var instance in instances)
                foreach (var @interface in instance.GetType().GetInterfaces())
                    services.AddSingleton(@interface, instance);
        }

        public static void InjectTransients<T>(IServiceCollection services)
        {
            var assembly = typeof(DependenceInjection).Assembly;
            var instances = assembly.DefinedTypes
                                    .Where(w => !w.IsInterface && !w.IsAbstract)
                                    .Where(w => w.GetInterface(typeof(T).Name) != null)
                                    .ToList();

            foreach (var instance in instances)
                    services.AddTransient(typeof(T), instance);
        }
    }
}
