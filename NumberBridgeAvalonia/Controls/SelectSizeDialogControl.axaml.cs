using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace NumberBridgeAvalonia.Controls;

public partial class SelectSizeDialogControl : UserControl
{
    public SelectSizeDialogControl()
    {
        InitializeComponent();
        Slider_Width.Minimum = 6;
        Slider_Width.Maximum = 50;
        Slider_Length.Minimum = 6;
        Slider_Length.Maximum = 50;
    }
    private Action<bool>? listener=null;

    public void SetListener(Action<bool> listener)
    {
        this.listener = listener;
    }

    public void GetSize(out int width, out int length)
    {
        width = (int)Slider_Width.Minimum;
        length = (int)Slider_Length.Minimum;
        if (string.IsNullOrEmpty(TextBox_Custom.Text))
        {
            width = (int)Slider_Width.Value;
            length = (int)Slider_Length.Value;
            return;
        }

        if (int.TryParse(TextBox_Custom.Text, out int custom))
        {
            width = custom;
            length = custom;
            return;
        }

        if (TextBox_Custom.Text.Contains(','))
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

    private void Button_Confirm_Click(object? sender, RoutedEventArgs e)
    {
        listener?.Invoke(true);
    }

    private void Button_Cancel_Click(object? sender, RoutedEventArgs e)
    {
        listener?.Invoke(false);
    }
}