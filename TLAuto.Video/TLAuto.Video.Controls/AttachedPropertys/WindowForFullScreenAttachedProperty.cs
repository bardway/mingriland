// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;
#endregion

namespace TLAuto.Video.Controls.AttachedPropertys
{
    public class WindowForFullScreenAttachedProperty : DependencyObject
    {
        private static WindowState _defaultWindowState;
        private static WindowStyle _defaultWindowStyle;
        private static ResizeMode _defaultResizeMode;
        private static double _defaultLeft;
        private static double _defaultTop;
        private static double _defaultWidth;
        private static double _defaultHeight;

        #region Dependency Properties
        public static readonly DependencyProperty VideoWindowTypeProperty =
            DependencyProperty.RegisterAttached("VideoWindowType",
                                                typeof(VideoWindowType),
                                                typeof(WindowForFullScreenAttachedProperty),
                                                new PropertyMetadata(VideoWindowType.Default, OnVideoWindowTypeCallbackChanged));

        private static void OnVideoWindowTypeCallbackChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var element = obj as Window;
            if (element != null)
            {
                var videoWindowType = (VideoWindowType)e.NewValue;
                switch (videoWindowType)
                {
                    case VideoWindowType.Default:
                        element.WindowState = _defaultWindowState;
                        element.WindowStyle = _defaultWindowStyle;
                        element.ResizeMode = _defaultResizeMode;
                        element.Topmost = false;
                        element.Left = _defaultLeft;
                        element.Top = _defaultTop;
                        element.Width = _defaultWidth;
                        element.Height = _defaultHeight;
                        break;
                    case VideoWindowType.FullScreen:
                        _defaultWindowState = element.WindowState;
                        _defaultWindowStyle = element.WindowStyle;
                        _defaultResizeMode = element.ResizeMode;
                        _defaultLeft = element.Left;
                        _defaultTop = element.Top;
                        _defaultWidth = element.Width;
                        _defaultHeight = element.Height;

                        element.WindowState = WindowState.Maximized;
                        element.WindowStyle = WindowStyle.None;
                        element.ResizeMode = ResizeMode.NoResize;
                        element.Topmost = true;
                        element.Left = 0.0;
                        element.Top = 0.0;
                        element.Width = SystemParameters.PrimaryScreenWidth;
                        element.Height = SystemParameters.PrimaryScreenHeight;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static VideoWindowType GetVideoWindowType(DependencyObject obj)
        {
            return (VideoWindowType)obj.GetValue(VideoWindowTypeProperty);
        }

        public static void SetVideoWindowType(DependencyObject obj, double value)
        {
            obj.SetValue(VideoWindowTypeProperty, value);
        }
        #endregion
    }
}