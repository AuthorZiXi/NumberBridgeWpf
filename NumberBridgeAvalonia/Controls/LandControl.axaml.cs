using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NumberBridge;

namespace NumberBridgeAvalonia.Controls;
public partial class LandControl : UserControl
{
    public LandControl()
    {
        InitializeComponent();
    }
    public Land? TargetLand { get;set; }

    private void landButton_Click(object sender, RoutedEventArgs e)
    {
        if (TargetLand != null)
            LandButtonClickEvent?.Invoke(this, TargetLand);
    }
    public delegate void LandButtonClick(object sender, Land TargetLand);
    public event LandButtonClick? LandButtonClickEvent;
}
