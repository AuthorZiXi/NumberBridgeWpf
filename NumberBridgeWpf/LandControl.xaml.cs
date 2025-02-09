using NumberBridge;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumberBridgeWpf
{
    /// <summary>
    /// LandControl.xaml 的交互逻辑
    /// </summary>
    public partial class LandControl : UserControl
    {
        public LandControl()
        {
            InitializeComponent();
        }

        public Land? TargetLand
        {
            get { return (Land?)GetValue(TargetLandProperty); }
            set { SetValue(TargetLandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetLand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetLandProperty =
            DependencyProperty.Register("TargetLand", typeof(Land), typeof(LandControl),new(OnTargetLandChanged));
        private static void OnTargetLandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var landControl = (LandControl)d;
            var land = (Land)e.NewValue;
            landControl.TargetLand = land;
            landControl.landButton.Content = land.Number.ToString();
        }

        private void landButton_Click(object sender, RoutedEventArgs e)
        {
            if (TargetLand != null)
            LandButtonClickEvent?.Invoke(this, TargetLand);
        }
        public delegate void LandButtonClick(object sender, Land TargetLand);
        public event LandButtonClick? LandButtonClickEvent;
    }
}
