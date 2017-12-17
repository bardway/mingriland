// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Device.Controls.RichTextBoxEx
{
    /// <summary>
    /// RichTextBoxControlEx.xaml 的交互逻辑
    /// </summary>
    public partial class RichTextBoxControlEx : UserControl
    {
        public RichTextBoxControlEx()
        {
            InitializeComponent();
            RichT.Document.Blocks.Clear();
            RichT.Document.LineHeight = 3;
        }

        private void NotificationReceived(StatusNotificationMessage msg)
        {
            var paragraph = new Paragraph();
            var iconRun = new Run
                          {
                              Foreground = Brushes.Green,
                              FontWeight = FontWeights.Bold
                          };
            var textRun = new Run(DateTime.Now + ": " + msg.Notification)
                          {
                              Foreground = Brushes.White
                          };
            switch (msg.StatusType)
            {
                case StatusNotificationType.RInfo:
                    iconRun.Text = "√ ";
                    break;
                case StatusNotificationType.Error:
                    iconRun.Text = "X ";
                    iconRun.Foreground = Brushes.Red;
                    textRun.Foreground = Brushes.Red;
                    break;
            }
            if (msg.StatusType != StatusNotificationType.NInfo)
            {
                paragraph.Inlines.Add(iconRun);
            }
            paragraph.Inlines.Add(textRun);
            RichT.Document.Blocks.Add(paragraph);
            RichT.ScrollToEnd();

            MsgLines++;

            //SaveLog(msg.Notification);
            if (MsgLines == 1000)
            {
                MsgLines = 0;
                RichT.Document.Blocks.Clear();
            }
        }

        #region Dependency Properties
        public static readonly DependencyProperty MsgIdProperty =
            DependencyProperty.Register("MsgId", typeof(string), typeof(RichTextBoxControlEx), new PropertyMetadata(Guid.NewGuid().ToString(), OnMsgIdCallBack));

        private void OnMsgIdCallBack(string oldMsgId, string newMsgId)
        {
            if (!oldMsgId.IsNullOrEmpty())
            {
                Messenger.Default.Unregister<StatusNotificationMessage>(this);
            }
            if (!newMsgId.IsNullOrEmpty())
            {
                Messenger.Default.Register<StatusNotificationMessage>(this, newMsgId, notificationMessage => { DispatcherHelper.CheckBeginInvokeOnUI(() => { NotificationReceived(notificationMessage); }); });
            }
        }

        private static void OnMsgIdCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RichTextBoxControlEx)d).OnMsgIdCallBack(e.OldValue as string, e.NewValue as string);
        }

        public string MsgId { get => (string)GetValue(MsgIdProperty); set => SetValue(MsgIdProperty, value); }

        public static readonly DependencyProperty MsgLinesProperty =
            DependencyProperty.Register("MsgLines", typeof(int), typeof(RichTextBoxControlEx), new PropertyMetadata(0));

        public int MsgLines { get => (int)GetValue(MsgLinesProperty); set => SetValue(MsgLinesProperty, value); }
        #endregion
    }
}