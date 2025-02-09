using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NumberBridgeWpf
{
    /// <summary>
    /// SelectSizeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectSizeDialog : Window
    {
        public SelectSizeDialog(int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            InitializeComponent();
            Slider_Width.Minimum = minWidth;
            Slider_Width.Maximum = maxWidth;
            Slider_Length.Minimum = minHeight;
            Slider_Length.Maximum = maxHeight;
        }

        public void GetSize(out int width,out int length)
        {
            width = (int)Slider_Width.Minimum;
            length = (int)Slider_Length.Minimum;
            if (string.IsNullOrEmpty(TextBox_Custom.Text))
            {
                width = (int)Slider_Width.Value;
                length = (int)Slider_Length.Value;
                return;
            }
            else
            {
                if (int.TryParse(TextBox_Custom.Text, out int custom))
                {
                    width = custom;
                    length = custom;
                    return;
                }
                else if (TextBox_Custom.Text.Contains(','))
                {
                    var strs = TextBox_Custom.Text.Split(',');
                    if (strs.Length == 2)
                    {
                        if (int.TryParse(strs[0], out int width2) && int.TryParse(strs[1], out int length2))
                        {
                            width = width2;
                            length = length2;
                            return;
                        }
                    }
                }
            }
        }

        private void Button_Confirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 这竟然是一个巨坑，给DialogResult赋值会触发Closing事件，如果把e.Cancel设为true，就无法赋值成功导致ShowDialog返回值为空。
            // 官方文档竟然没有任何说明。。。。幸好我搜了一下
            //e.Cancel = true; 
            //if (DialogResult != true)
            //DialogResult = false;
            //this.Hide();
        }
    }
}
