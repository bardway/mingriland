// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections;
using System.Windows;
using System.Windows.Controls;

using TLAuto.Machine.Controls.Models.Enums;
#endregion

namespace TLAuto.Machine.Controls
{
    public class MachineControl : UserControl
    {
        #region Fields
        private const string TpTxtlogPart = "TxtLog";
        #endregion

        #region Properties
        private RichTextBox TxtLog { set; get; }
        #endregion

        #region Ctor
        static MachineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MachineControl), new FrameworkPropertyMetadata(typeof(MachineControl)));
        }

        public MachineControl()
        {
            Loaded += delegate { ApplyTemplate(); };
        }
        #endregion

        #region Dependency Properties
        #region DifficulyType
        public static readonly DependencyProperty DifficulyTypeProperty =
            DependencyProperty.Register("DifficulyType", typeof(DifficulySystemType), typeof(MachineControl), new PropertyMetadata(DifficulySystemType.Low));

        public DifficulySystemType DifficulyType { get => (DifficulySystemType)GetValue(DifficulyTypeProperty); set => SetValue(DifficulyTypeProperty, value); }
        #endregion

        #region AppParamInfo
        public static readonly DependencyProperty SwitchItemsSourceProperty =
            DependencyProperty.Register("SwitchItemsSource", typeof(IEnumerable), typeof(MachineControl));

        public IEnumerable SwitchItemsSource { get => (IEnumerable)GetValue(SwitchItemsSourceProperty); set => SetValue(SwitchItemsSourceProperty, value); }

        public static readonly DependencyProperty RelayItemsSourceProperty =
            DependencyProperty.Register("RelayItemsSource", typeof(IEnumerable), typeof(MachineControl));

        public IEnumerable RelayItemsSource { get => (IEnumerable)GetValue(RelayItemsSourceProperty); set => SetValue(RelayItemsSourceProperty, value); }
        #endregion
        #endregion

        #region Methods
        public override void OnApplyTemplate()
        {
            InitTxtLog();
            base.OnApplyTemplate();
        }

        private void InitTxtLog()
        {
            if (TxtLog == null)
            {
                TxtLog = Template.FindName(TpTxtlogPart, this) as RichTextBox;
                //TxtLog.Document = new System.Windows.Documents.FlowDocument();
            }
        }

        public void LogToUi(string msg)
        {
            TxtLog.AppendText(msg + "\n");
            TxtLog.ScrollToEnd();
        }
        #endregion
    }
}