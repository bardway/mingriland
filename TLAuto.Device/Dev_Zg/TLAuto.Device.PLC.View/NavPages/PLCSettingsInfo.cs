// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using TLAuto.BaseEx.Extensions;
using TLAuto.Device.Controls.NavFrame;
#endregion

namespace TLAuto.Device.PLC.View.NavPages
{
    public class PLCSettingsInfo : ISettingsParam
    {
        private PLCInfo _current;

        public PLCSettingsInfo(bool isEdit = false, PLCInfo editInfo = null)
        {
            IsEdit = isEdit;
            if (IsEdit)
            {
                Current = editInfo;
            }
        }

        public PLCInfo Current { private set => _current = value; get => _current ?? (_current = new PLCInfo()); }

        internal bool IsEdit { get; }

        public Uri PageUri { get; } = new Uri(UIExtensions.GetXamlUrl(typeof(PLCSettingsPage)), UriKind.Relative);

        public string HeaderName { get; } = "工控板配置";
    }
}