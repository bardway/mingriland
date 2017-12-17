// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
using System.Windows;
#endregion

namespace TLAuto.Device.Controls.Dialogs
{
    /// <summary>
    /// RemoveItemsView.xaml 的交互逻辑
    /// </summary>
    public partial class RemoveItemsView : Window
    {
        public RemoveItemsView(string title, IEnumerable<string> removeItems)
        {
            InitializeComponent();
            Title = title;
            LstControl.ItemsSource = removeItems;
        }

        public List<string> SelectedItems { get; } = new List<string>();

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LstControl.SelectedItems.Count > 0)
            {
                SelectedItems.Clear();
                foreach (var selectedItem in LstControl.SelectedItems)
                {
                    SelectedItems.Add(selectedItem.ToString());
                }
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("至少选择一项进行删除。");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}