// Criado por Jone Polvora
// 09 06 2012

using System;

namespace MEFedIoC
{
    public static class MefContainerExtensions
    {
        public static T Resolve<T>(this IMefContainer container)
        {
            var service = container.Resolve(typeof(T), null);
            return service is T ? (T)service : default(T);
        }

        public static T GetService<T>(this IServiceProvider provider)
        {
            var service = provider.GetService(typeof(T));
            return service is T ? (T)service : default(T);
        }

        public static void RegisterInstance<T>(this IMefContainer container, T instance)
        {
            container.RegisterInstance<T>(null, instance);
        }


        /// <summary>
        /// Requires T2 having parameterless constructor
        /// </summary>
        public static void Register<T, T2>(this IMefContainer container) where T2 : T, new()
        {
            container.RegisterFunc(() => new T2());
            container.RegisterFunc<T>(() => container.Resolve<T2>());
        }
    }
}