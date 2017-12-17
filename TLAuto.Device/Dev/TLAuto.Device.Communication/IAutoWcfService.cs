/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLAuto.Device.Communication.Events;

namespace TLAuto.Device.Communication
{
    public interface IAutoWcfService
    {
        Task<bool> StartWcfService(string serviceAddress, Type typeclass, IEnumerable<Type> typeInterfaces);

        void StopWcfService();

        event EventHandler<AutoWcfServiceErrorMessageEventArgs> Error;

        event EventHandler<AutoWcfServiceMessageEventArgs> Info;
    }
}