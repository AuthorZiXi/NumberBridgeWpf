using NumberBridge;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NumberBridgeWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        private string titleAddContent = "";
        private DispatcherTimer? _timer;  // 定时器
        private DateTime? _startTime;     // 计时开始时间
        private TimeSpan? _elapsedTime;   // 记录已用时间
        private bool gameEnd = true;
        public void UpdateTitle()
        {
            Title = "Number Bridge Wpf " + titleAddContent +" - By AuthorZiXi";
        }
        private NumberBridgeGamePanel? gamePanel;
        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            if (!gameEnd)
            {
                var result = MessageBox.Show("是否要放弃当前游戏？", "游戏未结束", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            var sizeDialog = new SelectSizeDialog(6, 6, 50, 50);
            var ok = sizeDialog.ShowDialog();
            if (ok != true) return;
            sizeDialog.GetSize(out int width, out int height);
            if (gamePanel == null)
            {
                gamePanel = new NumberBridgeGamePanel(width,height);
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

        private void UpdateTime(object? sender, EventArgs e)
        {
            _elapsedTime = DateTime.Now - _startTime;  // 计算经过的时间
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
            
            UpdateTitle();  // 更新窗口标题
        }

        private void RefreshGame()
        {
            if (gamePanel == null) return;
            PutGame.Put(gamePanel, GameCanvas,
                new SolidColorBrush(Colors.White),
                Background,
                new SolidColorBrush(Colors.White),
                new SolidColorBrush(Colors.White),
                LandButtonClick);
        }
        private LandControl? selectLand;
        private void LandButtonClick(object sender,Land land)
        {
            if (gamePanel == null) return;
            //MessageBox.Show(gamePanel.GetLandPos(land).ToString());
            if (selectLand != null)
            {
                var startLand = selectLand.TargetLand;
                if (startLand == null) return;
                if (startLand == land)
                {
                    selectLand.landButton.Background =Background;
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
                            var success =gamePanel.RemoveBridge(startLand, land);
                            if (!success)
                            {
                                MessageBox.Show("无法移除桥梁，可能两岛之间并非横向或纵向相邻，或存在中间岛", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                                nowselectLand.landButton.Background = Background;
                                selectLand.landButton.Background = Background;
                                selectLand = null;
                                UpdateGreenButton();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("无法建立桥梁，可能两岛之间并非横向或纵向相邻、存在中间岛、或者两岛之间的桥梁数量已达到上限", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            nowselectLand.landButton.Background = Background;
                            selectLand.landButton.Background = Background;
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

        private void solveButton_Click(object sender, RoutedEventArgs e)
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

        private void clearBridgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (gamePanel != null)
            {
                gamePanel.ClearBridges();
                RefreshGame();
            }
            UpdateProgress();
        }

        private void checkButton_Click(object sender, RoutedEventArgs e)
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
                                if (land.landButton.Background == Background)
                                land.landButton.Background = new SolidColorBrush(Color.FromRgb(85, 177, 85));
                            }
                            else if (land.landButton.Background != Background)
                            {
                                land.landButton.Background = Background;
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
                    MessageBox.Show("恭喜你，成功求解！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                }
                else if (needShowNotSolved)
                {
                    MessageBox.Show("该局尚未求解，请继续游戏", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            UpdateProgress();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!gameEnd)
            {
                var result = MessageBox.Show("游戏尚未结束，确定要退出吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
        private void UpdateProgress()
        {
            if (gamePanel != null)
            {
                gamePanel.GetProgress(out var progress,out var total);
                progressBar.Maximum = total;
                progressBar.Value = progress;

            }
        }
    }
}