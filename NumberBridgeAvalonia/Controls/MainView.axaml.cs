using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using NumberBridge;
using NumberBridgeAvalonia.Controls;
using NumberBridgeAvalonia.Views;

namespace NumberBridgeAvalonia.Controls;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private string titleAddContent = "";
    private DispatcherTimer? _timer; // 定时器
    private DateTime? _startTime; // 计时开始时间
    private TimeSpan? _elapsedTime; // 记录已用时间
    private bool gameEnd = true;

    public void UpdateTitle()
    {
        Title.Text = titleAddContent;
    }

    private NumberBridgeGamePanel? gamePanel;

    private async void newButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!gameEnd)
        {
            var result = await MessageBoxManager
                .GetMessageBoxStandard("游戏未结束", "是否要放弃当前游戏？", ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Question)
                .ShowAsync();
            if (result == ButtonResult.No)
            {
                return;
            }
        }

        sizeDialogControl.SetListener((ok) =>
        {
            if (ok)
            {
                newGame();
            }

            DialogPanel.IsVisible = false;
            appPanel.IsEnabled = true;
        });
        appPanel.IsEnabled = false;
        DialogPanel.IsVisible = true;

        void newGame()
        {
            sizeDialogControl.GetSize(out int width, out int height);
            if (gamePanel == null)
            {
                gamePanel = new NumberBridgeGamePanel(width, height);
            }
            else
            {
                gamePanel.PanelHeight = height;
                gamePanel.PanelWidth = width;
            }

            gamePanel.CreateNewGame();
            GameCanvas.IsEnabled = true;
            GameCanvas.Children.Clear();
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            RefreshGame();
            _startTime = DateTime.Now;
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += UpdateTime;
            }

            _timer.Start();
            solveButton.IsEnabled = true;
            checkButton.IsEnabled = true;
            clearBridgeButton.IsEnabled = true;
            gameEnd = false;
        }
        
    }
    

    private void UpdateTime(object? sender, EventArgs e)
    {
        _elapsedTime = DateTime.Now - _startTime; // 计算经过的时间
        if (_elapsedTime.HasValue)
        {
            var et = _elapsedTime.Value;
            if (et.Days > 0)
            {
                titleAddContent = $"已用时间： {et:d\\ day hh\\:mm\\:ss}";
            }
            else
            {
                titleAddContent = $"已用时间： {et:hh\\:mm\\:ss}";
            }
        }

        UpdateTitle(); // 更新窗口标题
    }
    
    private void RefreshGame()
    {
        if (CanvasNowSize == null) return;
        if (gamePanel == null) return;
        PutGame.Put(gamePanel, GameCanvas,
            CanvasNowSize.Value,
            new SolidColorBrush(Colors.White),
            new SolidColorBrush(Colors.Gray),
            new SolidColorBrush(Colors.White),
            new SolidColorBrush(Colors.White),
            LandButtonClick);
    }

    private LandControl? selectLand;
    private IBrush defaultLandBgBrush=new SolidColorBrush(Colors.Gray);
    private void LandButtonClick(object sender, Land land)
    {
        if (gamePanel == null) return;
        //MessageBox.Show(gamePanel.GetLandPos(land).ToString());
        if (selectLand != null)
        {
            var startLand = selectLand.TargetLand;
            //Debug.WriteLine(ActualThemeVariant.Key);
            if (startLand == null) return;
            if (startLand == land)
            {
                selectLand.landButton.Background = defaultLandBgBrush;
                selectLand = null;
                UpdateGreenButton();
            }
            else
            {
                var nowselectLand = (LandControl)sender;
                nowselectLand.landButton.Background = new SolidColorBrush(Colors.DarkCyan);
                var result = gamePanel.LinkBridge(startLand, land, out var needRemove);
                if (!result)
                {
                    if (needRemove)
                    {
                        var success = gamePanel.RemoveBridge(startLand, land);
                        if (!success)
                        {
                            MessageBoxManager
                                .GetMessageBoxStandard("错误","无法移除桥梁，可能两岛之间并非横向或纵向相邻，或存在中间岛",  ButtonEnum.Ok,
                                    MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
                            nowselectLand.landButton.Background = defaultLandBgBrush;
                            selectLand.landButton.Background = defaultLandBgBrush;
                            selectLand = null;
                            UpdateGreenButton();
                            return;
                        }
                    }
                    else
                    {
                        MessageBoxManager
                            .GetMessageBoxStandard("错误","无法建立桥梁，可能两岛之间并非横向或纵向相邻、存在中间岛、或者两岛之间的桥梁数量已达到上限",  ButtonEnum.Ok,
                                MsBox.Avalonia.Enums.Icon.Error).ShowAsync();
                        nowselectLand.landButton.Background = defaultLandBgBrush;
                        selectLand.landButton.Background = defaultLandBgBrush;
                        selectLand = null;
                        UpdateGreenButton();
                        return;
                    }
                }

                selectLand = null;
                RefreshGame();
                CheckSolved();
            }
        }
        else
        {
            selectLand = (LandControl)sender;
            selectLand.landButton.Background = new SolidColorBrush(Colors.DarkCyan);
        }
    }

    private void solveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (gamePanel != null)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }

            gamePanel.SolveGame();
            RefreshGame();
            GameCanvas.IsEnabled = false;
            solveButton.IsEnabled = false;
            checkButton.IsEnabled = false;
            clearBridgeButton.IsEnabled = false;
            gameEnd = true;
            titleAddContent = "该局已求解，将无法继续游戏";
            UpdateTitle();
            UpdateProgress();
        }
    }

    private void clearBridgeButton_Click(object? sender, RoutedEventArgs e)
    {
        if (gamePanel != null)
        {
            gamePanel.ClearBridges();
            RefreshGame();
        }

        UpdateProgress();
    }

    private void checkButton_Click(object? sender, RoutedEventArgs e)
    {
        CheckSolved(true);
    }

    private void UpdateGreenButton()
    {
        if (GameCanvas.Children.Count > 0)
        {
            foreach (var item in GameCanvas.Children)
            {
                if (item is LandControl)
                {
                    var land = item as LandControl;
                    if (land == null) continue;
                    if (land.TargetLand != null)
                    {
                        if (land.TargetLand.CanBuildBridgeCount() == 0)
                        {
                            if (land.landButton.Background == defaultLandBgBrush)
                                land.landButton.Background = new SolidColorBrush(Color.FromRgb(85, 177, 85));
                        }
                        else if (land.landButton.Background != defaultLandBgBrush)
                        {
                            land.landButton.Background = defaultLandBgBrush;
                        }
                    }
                }
            }
        }
    }

    private void CheckSolved(bool needShowNotSolved = false)
    {
        if (gamePanel != null)
        {
            UpdateGreenButton();
            var result = gamePanel.IsSolved();
            if (result)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                }

                GameCanvas.IsEnabled = false;
                titleAddContent = "恭喜你，该局获胜！";
                UpdateTitle();
                solveButton.IsEnabled = false;
                checkButton.IsEnabled = false;
                clearBridgeButton.IsEnabled = false;
                gameEnd = true;
                MessageBoxManager
                    .GetMessageBoxStandard( "成功","恭喜你，成功求解！", ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info)
                    .ShowAsync();
            }
            else if (needShowNotSolved)
            {
                MessageBoxManager
                    .GetMessageBoxStandard("提示","该局尚未求解，请继续游戏",  ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Info)
                    .ShowAsync();
            }
        }

        UpdateProgress();
    }

    private async void exitButton_Click(object sender, RoutedEventArgs e)
    {
        if (await Window_Closing())
        {
            Environment.Exit(0);         
        }
    }

    public async Task<bool> Window_Closing()
    {
        if (!gameEnd)
        {
            var result = await MessageBoxManager
                .GetMessageBoxStandard("提示","游戏尚未结束，确定要退出吗？", ButtonEnum.YesNo, MsBox.Avalonia.Enums.Icon.Question)
                .ShowAsync();
            if (result == ButtonResult.No)
            {
                return false;
            }
            Environment.Exit(0);
        }
        return true;
    }

    private void UpdateProgress()
    {
        if (gamePanel != null)
        {
            gamePanel.GetProgress(out var progress, out var total);
            progressBar.Maximum = total;
            progressBar.Value = progress;
        }
    }

    private Size? CanvasNowSize;
    private void GameCanvas_OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        Debug.WriteLine(e.NewSize);
        CanvasNowSize = e.NewSize;
    }
}