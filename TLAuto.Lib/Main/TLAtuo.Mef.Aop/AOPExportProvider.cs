// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
#endregion

namespace TLAtuo.Mef.Aop
{
    public class AOPExportProvider : ExportProvider, IDisposable
    {
        private readonly CatalogExportProvider _exportProvider;

        public AOPExportProvider(Func<ComposablePartCatalog> catalogResolver)
        {
            _exportProvider = new CatalogExportProvider(catalogResolver());

            //support recomposition
            _exportProvider.ExportsChanged += (s, e) => OnExportsChanged(e);
            _exportProvider.ExportsChanging += (s, e) => OnExportsChanging(e);
        }

        public ExportProvider SourceProvider { get => _exportProvider.SourceProvider; set => _exportProvider.SourceProvider = value; }

        public void Dispose()
        {
            _exportProvider.Dispose();
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            var exports = _exportProvider.GetExports(definition, atomicComposition);
            return exports.Select(export => new Export(export.Definition, () => GetValue(export)));
        }

        private object GetValue(Export innerExport)
        {
            var value = innerExport.Value;
            if (innerExport.Metadata.Any(x => x.Key == "AreAspectsEnabled"))
            {
                var specificMetadata = innerExport.Metadata.Single(x => x.Key == "AreAspectsEnabled");
                if ((bool)specificMetadata.Value)
                {
                    return AspectProxy.Factory(value);
                }
                return value;
            }
            return value;
        }
    }
}