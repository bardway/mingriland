// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
#endregion

namespace TLAuto.Machine.Plugins.Core
{
    public sealed class MachineBuilder
    {
        public static readonly MachineBuilder Instance = new MachineBuilder();
        private CompositionContainer _container;

        private MachineBuilder() { }

        public IMachinePluginsProvider MachinePluginsProvider { private set; get; }

        public void Init(string key)
        {
            var aggregateCatalog = new AggregateCatalog();
            var thisAssembly = new DirectoryCatalog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MachinePlugins"), "*.dll");
            aggregateCatalog.Catalogs.Add(thisAssembly);
            _container = new CompositionContainer(aggregateCatalog);
            _container.ComposeParts(this);

            MachinePluginsProvider = _container.GetExportedValue<IMachinePluginsProvider>(key);
        }
    }
}