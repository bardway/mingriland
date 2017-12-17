// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows.Controls;
using System.Windows.Navigation;

using GalaSoft.MvvmLight;
#endregion

namespace TLAuto.Device.Controls.NavFrame
{
    public sealed class NavFrameInfo : ObservableObject
    {
        private Frame _frame;

        public NavFrameInfo(ISettingsParam paramObj)
        {
            PageUri = paramObj.PageUri;
            NavTitle = paramObj.HeaderName;
            ParamObj = paramObj;
        }

        internal void Show(Frame frame)
        {
            if (_frame == null)
            {
                frame.Navigating += Frame_Navigating;
                frame.Navigated += Frame_Navigated;
            }
            frame.Navigate(PageUri, ParamObj);
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            IsShow = e.Uri.OriginalString == PageUri.OriginalString;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.OriginalString == PageUri.OriginalString)
            {
                if (_frame == null)
                {
                    var notifyNavStatusChanged = e.Content as INotifyNavStatusChanged;
                    if (notifyNavStatusChanged != null)
                    {
                        _frame = (Frame)sender;
                        notifyNavStatusChanged.NavStatusChanged += NavFrameInfo_NavStatusChanged;
                        notifyNavStatusChanged.ParamObj = ParamObj;
                    }
                }
            }
        }

        private void NavFrameInfo_NavStatusChanged(object sender, NavStatusChangedEventArgs e)
        {
            HasCompletedSettings = e.IsCompleted;
            if (e.IsCompleted)
            {
                OnNavCompleted();
            }
            else
            {
                OnNavFailed();
            }
        }

        #region Events
        internal event EventHandler NavCompleted;

        private void OnNavCompleted()
        {
            NavCompleted?.Invoke(this, EventArgs.Empty);
        }

        internal event EventHandler NavFailed;

        private void OnNavFailed()
        {
            NavFailed?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Properties
        public Uri PageUri { get; }

        public string NavTitle { get; }

        private bool _isShow;

        public bool IsShow
        {
            private set
            {
                _isShow = value;
                RaisePropertyChanged();
            }
            get => _isShow;
        }

        public object ParamObj { get; }

        public bool HasCompletedSettings { private set; get; }
        #endregion
    }
}