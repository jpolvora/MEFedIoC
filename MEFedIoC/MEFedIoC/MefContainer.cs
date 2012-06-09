using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace MEFedIoC
{
    public class MefContainer : CompositionContainer, IMefContainer
    {
        public MefContainer(ComposablePartCatalog catalog)
            : base(catalog)
        {
        }

        #region Implementation of IServiceProvider

        public object GetService(Type serviceType)
        {
            return Resolve(serviceType, null);
        }

        #endregion

        #region Implementation of IMefContainer

        public object Resolve(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key)
                               ? AttributedModelServices.GetContractName(serviceType)
                               : key;
            var exports = GetExportedValues<object>(contract);
            return exports.FirstOrDefault();
        }

        public void RegisterFunc<T>(Func<T> functor)
        {
            var batch = new CompositionBatch();
            var contractName = AttributedModelServices.GetContractName(typeof(T));
            var typeIdentity = AttributedModelServices.GetTypeIdentity(typeof(T));
            IDictionary<string, object> metadata = new Dictionary<string, object>();
            metadata.Add("ExportTypeIdentity", typeIdentity);
            batch.AddExport(new Export(contractName, metadata, () => functor()));
            Compose(batch);
        }

        public void RegisterInstance<T>(string key, T instance)
        {
            var contract = string.IsNullOrEmpty(key)
                               ? AttributedModelServices.GetContractName(typeof(T))
                               : key;

            var batch = new CompositionBatch();
            batch.AddExportedValue(contract, instance);

            Compose(batch);
        }

        public void BuildUp(object instance)
        {
            this.SatisfyImportsOnce(instance);
        }

        public void Compose(params object[] attrParts)
        {
            this.ComposeParts(attrParts);
        }

        #endregion
    }
}