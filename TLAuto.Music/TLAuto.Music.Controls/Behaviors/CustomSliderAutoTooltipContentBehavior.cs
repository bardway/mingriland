// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
#endregion

namespace TLAuto.Music.Controls.Behaviors
{
    public class CustomSliderAutoTooltipContentBehavior : Behavior<Slider>
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(CustomSliderAutoTooltipContentBehavior), new PropertyMetadata(string.Empty));

        private ToolTip _autoToolTip;

        public string Content { get => (string)GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        private ToolTip AutoToolTip
        {
            get
            {
                if (_autoToolTip == null)
                {
                    var field = typeof(Slider).GetField("_autoToolTip", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null)
                    {
                        _autoToolTip = field.GetValue(AssociatedObject) as ToolTip;
                    }
                }
                return _autoToolTip;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(MediaProgress_DragStarted));
            AssociatedObject.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(MediaProgress_DragDelta));
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(MediaProgress_DragStarted));
            AssociatedObject.RemoveHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(MediaProgress_DragDelta));
        }

        private void MediaProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            SetAutoToolTipContent();
        }

        private void MediaProgress_DragDelta(object sender, DragDeltaEventArgs e)
        {
            SetAutoToolTipContent();
        }

        private void SetAutoToolTipContent()
        {
            if (AutoToolTip != null)
            {
                AutoToolTip.Content = Content;
            }
        }
    }
}