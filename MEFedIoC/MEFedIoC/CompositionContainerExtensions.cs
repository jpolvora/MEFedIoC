using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace MEFedIoC
{
    public static class CompositionContainerExtensions
    {
        public static object Resolve(this CompositionContainer container, Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key)
                               ? AttributedModelServices.GetContractName(serviceType)
                               : key;

            var exports = container.GetExportedValues<object>(contract);
            return exports.FirstOrDefault();
        }

        public static T Resolve<T>(this CompositionContainer container)
        {
            var service = container.Resolve(typeof(T), null);
            return service is T ? (T)service : default(T);
        }

        public static void RegisterFunc<T>(this CompositionContainer container, Func<T> functor)
        {
            var batch = new CompositionBatch();
            var contractName = AttributedModelServices.GetContractName(typeof(T));
            var typeIdentity = AttributedModelServices.GetTypeIdentity(typeof(T));
            IDictionary<string, object> metadata = new Dictionary<string, object>();
            metadata.Add("ExportTypeIdentity", typeIdentity);
            batch.AddExport(new Export(contractName, metadata, () => functor()));
            container.Compose(batch);
        }

        public static void RegisterInstance<T>(this CompositionContainer container, T instance)
        {
            container.RegisterInstance(null, instance);
        }

        public static void RegisterInstance<T>(this CompositionContainer container, string key, T instance)
        {
            var contract = string.IsNullOrEmpty(key)
                               ? AttributedModelServices.GetContractName(typeof(T))
                               : key;

            var batch = new CompositionBatch();
            batch.AddExportedValue(contract, instance);

            container.Compose(batch);
        }

        public static void BuildUp(this CompositionContainer container, object instance)
        {
            container.SatisfyImportsOnce(instance);
        }

        /// <summary>
        /// Requires T2 having parameterless constructor
        /// </summary>
        public static void Register<T, T2>(this CompositionContainer container) where T2 : T, new()
        {
            container.RegisterFunc(() => new T2());
            container.RegisterFunc<T>(() => container.Resolve<T2>());
        }

        public static void Compose(this CompositionContainer container, params object[] attrParts)
        {
            container.ComposeParts(attrParts);
        }
    }
}