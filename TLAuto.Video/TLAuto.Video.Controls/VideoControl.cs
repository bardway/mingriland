// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
#endregion

namespace TLAuto.Video.Controls
{
    public class VideoControl : Control
    {
        static VideoControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoControl), new FrameworkPropertyMetadata(typeof(VideoControl)));
        }

        public VideoControl()
        {
            Loaded += delegate { ApplyTemplate(); };
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += Timer_Tick;
        }

        #region Properties
        private MediaElement Element { set; get; }
        #endregion

        private void Timer_Tick(object sender, EventArgs e)
        {
            var totalSeconds = Element.Position.TotalSeconds;
            if ((_pauseEventTime > 0) && (totalSeconds >= _pauseEventTime))
            {
                Element.Pause();
                _pauseEventTime = 0;
            }
            Debug.WriteLine(totalSeconds);
        }

        private void InitMediaElement()
        {
            if (Element == null)
            {
                Element = Template.FindName(TpElementPart, this) as MediaElement;
                if (Element != null)
                {
                    Element.MediaOpened += Element_MediaOpened;
                    Element.MediaEnded += Element_MediaEnded;
                }
            }
        }

        private void Element_MediaEnded(object sender, RoutedEventArgs e)
        {
            Element.Stop();
            if (IsRepeat)
            {
                Element.Play();
            }
        }

        private void Element_MediaOpened(object sender, RoutedEventArgs e)
        {
            Element.Stop();
            Element.Play();
        }

        public override void OnApplyTemplate()
        {
            InitMediaElement();
            base.OnApplyTemplate();
        }

        public void Play()
        {
            if ((Element != null) && (Source != null))
            {
                Element.Play();
            }
        }

        public void Pause()
        {
            if ((Element != null) && (Source != null))
            {
                Element.Pause();
            }
        }

        public void Stop()
        {
            if ((Element != null) && (Source != null))
            {
                Element.Stop();
            }
        }

        public void SetPauseTimeEvent(TimeSpan time)
        {
            _pauseEventTime = time.TotalSeconds;
        }

        public void ChangeFrame(TimeSpan time)
        {
            Element.Position = time;
        }

        #region Control Part constants
        private const string TpElementPart = "Media";
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private double _pauseEventTime;
        #endregion

        #region Dependency Properties
        #region Source
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(VideoControl), new PropertyMetadata(OnSourceCallbackChanged));

        private void OnSourceCallbackChanged(object newValue)
        {
            if (newValue != null)
            {
                Element?.Play();
                _timer.Start();
            }
        }

        private static void OnSourceCallbackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoControl)d).OnSourceCallbackChanged(e.NewValue);
        }

        public Uri Source { get => (Uri)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
        #endregion

        #region Volume
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(VideoControl), new PropertyMetadata(1.0));

        public double Volume { get => (double)GetValue(VolumeProperty); set => SetValue(VolumeProperty, value); }
        #endregion

        #region IsRepeat
        public static readonly DependencyProperty IsRepeatProperty =
            DependencyProperty.Register("IsRepeat", typeof(bool), typeof(VideoControl), new PropertyMetadata(false));

        public bool IsRepeat { get => (bool)GetValue(IsRepeatProperty); set => SetValue(IsRepeatProperty, value); }
        #endregion
        #endregion
    }
}