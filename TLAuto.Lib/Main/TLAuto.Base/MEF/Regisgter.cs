// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.ComponentModel.Composition.Hosting;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Base.MEF
{
    public class Regisgter
    {
        private static readonly object Obj = new object();

        public static CompositionContainer Reg(string path = "")
        {
            lock (Obj)
            {
                try
                {
                    var aggregateCatalog = new AggregateCatalog();
                    path = path.IsNullOrEmpty() ? AppDomain.CurrentDomain.BaseDirectory : path;
                    var thisAssembly = new DirectoryCatalog(path, "*.dll");
                    //if (!thisAssembly.Any())
                    //{
                    //    path = path + "bin\\";
                    //    thisAssembly = new DirectoryCatalog(path, "*.dll");
                    //}
                    aggregateCatalog.Catalogs.Add(thisAssembly);
                    var container = new CompositionContainer(aggregateCatalog);
                    return container;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}