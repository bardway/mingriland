// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Music.Controls
{
    public class MusicControl : Control
    {
        #region Fields
        private readonly MediaElement _media = new MediaElement();
        private readonly DispatcherTimer _mediaTimer = new DispatcherTimer();
        private bool _isDragging;
        private bool _isLoaded;

        #region Control Part constants
        private const string TP_PLAYBTN_PART = "PlayBtn";
        private const string TP_PAUSEBTN_PART = "PauseBtn";
        private const string TP_STOPBTN_PART = "StopBtn";
        private const string TP_MEDIAPROGRESS_PART = "MediaProgress";
        #endregion
        #endregion

        #region Dependency Properties
        #region Source
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(MusicControl), new PropertyMetadata(OnSourceChanged));

        private void OnSourceChanged()
        {
            if (_isLoaded)
            {
                LoadSource();
            }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MusicControl)d).OnSourceChanged();
        }

        public string Source { get => (string)GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
        #endregion

        #region Volume
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(MusicControl), new PropertyMetadata(0.5, OnVolumeChanged));

        private void OnVolumeChanged(double newValue)
        {
            _media.Volume = newValue;
        }

        private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MusicControl)d).OnVolumeChanged((double)e.NewValue);
        }

        public double Volume { get => (double)GetValue(VolumeProperty); set => SetValue(VolumeProperty, value); }
        #endregion

        #region SpeedRatio
        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(MusicControl), new PropertyMetadata(1.0, OnSpeedRatioChanged));

        private void OnSpeedRatioChanged(double newValue)
        {
            _media.SpeedRatio = newValue;
        }

        private static void OnSpeedRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MusicControl)d).OnSpeedRatioChanged((double)e.NewValue);
        }

        public double SpeedRatio { get => (double)GetValue(SpeedRatioProperty); set => SetValue(SpeedRatioProperty, value); }
        #endregion

        #region IsRepeat
        public static readonly DependencyProperty IsRepeatProperty =
            DependencyProperty.Register("IsRepeat", typeof(bool), typeof(MusicControl), new PropertyMetadata(false));

        public bool IsRepeat { get => (bool)GetValue(IsRepeatProperty); set => SetValue(IsRepeatProperty, value); }
        #endregion

        #region MusicStatusType
        public static readonly DependencyProperty MusicStatusTypeProperty =
            DependencyProperty.Register("MusicStatusType", typeof(MusicStatusType), typeof(MusicControl), new PropertyMetadata(MusicStatusType.Stop, OnMusicStateTypeChanged));

        private void OnMusicStateTypeChanged(MusicStatusType statusType)
        {
            if (Source.IsNullOrEmpty())
            {
                return;
            }
            switch (statusType)
            {
                case MusicStatusType.Play:
                    _media.Play();
                    break;
                case MusicStatusType.Pause:
                    _media.Pause();
                    break;
                case MusicStatusType.Stop:
                    _media.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("statusType", statusType, null);
            }
        }

        private static void OnMusicStateTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MusicControl)d).OnMusicStateTypeChanged((MusicStatusType)e.NewValue);
        }

        public MusicStatusType MusicStatusType { get => (MusicStatusType)GetValue(MusicStatusTypeProperty); set => SetValue(MusicStatusTypeProperty, value); }
        #endregion
        #endregion

        #region Ctor
        static MusicControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MusicControl), new FrameworkPropertyMetadata(typeof(MusicControl)));
        }

        public MusicControl()
        {
            InitMedia();
            Loaded += delegate
                      {
                          ApplyTemplate();
                          _isLoaded = true;
                          if (!Source.IsNullOrEmpty())
                          {
                              LoadSource();
                          }
                      };
            Unloaded += delegate { Dispose(); };
        }
        #endregion

        #region Methods
        private void LoadSource()
        {
            if (Source.IsNullOrEmpty())
            {
                return;
            }
            Close();
            _media.Source = new Uri(Source, UriKind.Absolute);
            _media.Volume = 0;
            SetValue(MusicStatusTypeProperty, MusicStatusType.Play);
        }

        private void Dispose()
        {
            if (MediaProgress != null)
            {
                MediaProgress.RemoveHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(MediaProgress_DragStarted));
                MediaProgress.RemoveHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(MediaProgress_DragCompleted));
            }
            Close();
        }

        private void Close()
        {
            _mediaTimer.Stop();
            SetValue(MusicStatusTypeProperty, MusicStatusType.Stop);
            _media.Close();
        }

        private void InitMedia()
        {
            _media.LoadedBehavior = MediaState.Manual;
            _media.UnloadedBehavior = MediaState.Manual;
            _media.MediaOpened += Media_MediaOpened;
            _media.MediaEnded += Media_MediaEnded;
            _mediaTimer.Interval = TimeSpan.FromMilliseconds(200);
            _mediaTimer.Tick += MediaTimer_Tick;
        }

        public override void OnApplyTemplate()
        {
            InitPlayBtn();
            InitPauseBtn();
            InitStopBtn();
            InitMediaProgress();
            base.OnApplyTemplate();
        }

        private void InitPlayBtn()
        {
            if (PlayBtn == null)
            {
                PlayBtn = Template.FindName(TP_PLAYBTN_PART, this) as Button;
                if (PlayBtn != null)
                {
                    PlayBtn.Click += (s, e) => SetValue(MusicStatusTypeProperty, MusicStatusType.Play);
                }
            }
        }

        private void InitPauseBtn()
        {
            if (PauseBtn == null)
            {
                PauseBtn = Template.FindName(TP_PAUSEBTN_PART, this) as Button;
                if (PauseBtn != null)
                {
                    PauseBtn.Click += (s, e) => SetValue(MusicStatusTypeProperty, MusicStatusType.Pause);
                }
            }
        }

        private void InitStopBtn()
        {
            if (StopBtn == null)
            {
                StopBtn = Template.FindName(TP_STOPBTN_PART, this) as Button;
                if (StopBtn != null)
                {
                    StopBtn.Click += (s, e) => SetValue(MusicStatusTypeProperty, MusicStatusType.Stop);
                }
            }
        }

        private void InitMediaProgress()
        {
            if (MediaProgress == null)
            {
                MediaProgress = Template.FindName(TP_MEDIAPROGRESS_PART, this) as Slider;
                if (MediaProgress != null)
                {
                    MediaProgress.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(MediaProgress_DragStarted));
                    MediaProgress.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(MediaProgress_DragCompleted));
                }
            }
        }

        private void InitFileProperty()
        {
            _media.Volume = Volume;
            if (_media.NaturalDuration.HasTimeSpan)
            {
                var ts = _media.NaturalDuration.TimeSpan;
                MediaProgress.Maximum = _media.NaturalDuration.TimeSpan.TotalSeconds;
                MediaProgress.SmallChange = 1;
                MediaProgress.LargeChange = Math.Min(10, ts.Seconds / 10);
            }
        }

        private void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            SetValue(MusicStatusTypeProperty, MusicStatusType.Stop);
            InitFileProperty();
            _mediaTimer.Start();
            SetValue(MusicStatusTypeProperty, MusicStatusType.Play);
        }

        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            SetValue(MusicStatusTypeProperty, MusicStatusType.Stop);
            if (IsRepeat)
            {
                SetValue(MusicStatusTypeProperty, MusicStatusType.Play);
            }
        }

        private void MediaProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
        }

        private void MediaProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            if (_media.NaturalDuration.HasTimeSpan)
            {
                _media.Position = TimeSpan.FromSeconds(MediaProgress.Value);
            }
        }

        private void MediaTimer_Tick(object sender, EventArgs e)
        {
            if (!_isDragging)
            {
                MediaProgress.Value = _media.Position.TotalSeconds;
            }
        }
        #endregion

        #region Properties
        private Button PlayBtn { set; get; }

        private Button PauseBtn { set; get; }

        private Button StopBtn { set; get; }

        private Slider MediaProgress { set; get; }
        #endregion
    }
}