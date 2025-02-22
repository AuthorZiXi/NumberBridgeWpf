using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using NumberBridge;
using NumberBridgeAvalonia.Controls;

namespace NumberBridgeAvalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        Thread thread = new Thread(Ask);
        thread.Start();
        e.Cancel = true;
    }

    private async void Ask()
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            if (await MainView.Window_Closing())
            {
                Environment.Exit(0);
            }
        });
    }
}
