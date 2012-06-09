using System;

namespace MEFedIoC
{
    public interface IMefContainer : IServiceProvider
    {
        object Resolve(Type serviceType, string key);
        void RegisterFunc<T>(Func<T> functor);
        void RegisterInstance<T>(string key, T instance);
        void BuildUp(object instance);
    }
}