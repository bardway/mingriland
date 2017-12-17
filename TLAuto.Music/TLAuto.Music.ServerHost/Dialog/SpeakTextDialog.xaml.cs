// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Windows;

using TLAuto.Base.Extensions;
#endregion

namespace TLAuto.Music.ServerHost.Dialog
{
    /// <summary>
    /// SpeakTextDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SpeakTextDialog : Window
    {
        public SpeakTextDialog()
        {
            InitializeComponent();
        }

        public string SpeakText => TxtSpeak.Text.Trim();

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!SpeakText.IsNullOrEmpty())
            {
                DialogResult = true;
            }
            Close();
        }
    }
}