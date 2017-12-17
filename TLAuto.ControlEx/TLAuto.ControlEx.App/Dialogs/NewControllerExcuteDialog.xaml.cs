// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;

using TLAuto.ControlEx.App.Models.Enums;
#endregion

namespace TLAuto.ControlEx.App.Dialogs
{
    /// <summary>
    /// NewControllerExcuteDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewControllerExcuteDialog : Window
    {
        public NewControllerExcuteDialog()
        {
            InitializeComponent();
            var names = Enum.GetNames(typeof(ExcuteType));
            foreach (var name in names)
            {
                CboExcutes.Items.Add((ExcuteType)Enum.Parse(typeof(ExcuteType), name));
            }
            CboExcutes.SelectedIndex = 0;
        }

        public ExcuteType SelectedExcuteType => (ExcuteType)CboExcutes.SelectedItem;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (CboExcutes.SelectedIndex != -1)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}