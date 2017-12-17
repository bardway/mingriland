// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;

using TLAuto.BaseEx.Extensions;
using TLAuto.Device.Controls.NavFrame;
#endregion

namespace TLAuto.Device.Projector.View.NavPages
{
    public class ProjectorSettingsInfo : ISettingsParam
    {
        private ProjectorInfo _current;

        public ProjectorSettingsInfo(bool isEdit = false, ProjectorInfo editInfo = null)
        {
            IsEdit = isEdit;
            if (IsEdit)
            {
                Current = editInfo;
            }
        }

        public ProjectorInfo Current { private set => _current = value; get => _current ?? (_current = new ProjectorInfo()); }

        internal bool IsEdit { get; }

        public Uri PageUri { get; } = new Uri(UIExtensions.GetXamlUrl(typeof(ProjectorSettingsPage)), UriKind.Relative);

        public string HeaderName { get; } = "投影仪配置";
    }
}