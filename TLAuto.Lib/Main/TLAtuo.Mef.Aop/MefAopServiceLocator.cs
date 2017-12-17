// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
#endregion

namespace TLAtuo.Mef.Aop
{
    public sealed class MefAopServiceLocator
    {
        private static readonly Lazy<MefAopServiceLocator> _instance = new Lazy<MefAopServiceLocator>(() => new MefAopServiceLocator());
        private CompositionContainer _container;

        private MefAopServiceLocator() { }

        public static MefAopServiceLocator Instance => _instance.Value;

        public void Configure(Func<ComposablePartCatalog> catalogResolver)
        {
            var aopExportProvider = new AOPExportProvider(catalogResolver);
            _container = new CompositionContainer(aopExportProvider);
            aopExportProvider.SourceProvider = _container;
        }

        public T Resolve<T>()
        {
            return _container.GetExport<T>().Value;
        }
    }
}