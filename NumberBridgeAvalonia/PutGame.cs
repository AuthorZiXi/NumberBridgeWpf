using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using NumberBridge;
using NumberBridgeAvalonia.Controls;

namespace NumberBridgeAvalonia;

public static class PutGame
{
    public static void Put(NumberBridgeGamePanel panel, Canvas canvas,Size size,Brush landBrush,Brush landBgBrush,Brush bridgeBrush,Brush textBrush, LandControl.LandButtonClick? click=null)
    {
        canvas.Children.Clear();
        var items = panel.Items;  // 获取面板中的所有土地
        var whsmallone = Math.Min(size.Width, size.Height);
        var landSizeX = whsmallone / panel.PanelWidth;
        var landSizeY = whsmallone / panel.PanelHeight;
        var w = whsmallone;
        var h = whsmallone;
        var drawedBridges = new List<Bridge>();  // 存储已画过的桥梁

        // 设置绘图的笔刷和字体样式
        //var landBrush = new SolidColorBrush(Colors.White);
        //var textBrush = new SolidColorBrush(Colors.White);  // 文本颜色为黑色
        //var bridgeBrush = new SolidColorBrush(Colors.White);  // 桥梁颜色为白色
        //var fontSize = 12;
        //var font = new FontFamily("Arial");

        // 绘制方格（调试用）
#if DEBUG
        for (int i = 0; i < panel.PanelWidth; i++)
        {
            for (int j = 0; j < panel.PanelHeight; j++)
            {
                var rect = new Rectangle
                {
                    Width = landSizeX,
                    Height = landSizeY,
                    Stroke = Brushes.Pink,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(rect, i * landSizeX);
                Canvas.SetTop(rect, j * landSizeY);
                rect.ZIndex = -1;
                canvas.Children.Add(rect);
            }
        }
#endif
        // 绘制桥
        foreach (var item in items)
        {
            if (item.Value is Bridge bridge)
            {
                if (drawedBridges.Contains(bridge))
                {
                    continue;
                }
                drawedBridges.Add(bridge);
                //找到PortA和PortB在的坐标
                //由于items的键是坐标，所以需要循环遍历来知道
                double x1=0, y1=0, x2=0, y2=0;
                foreach (var item2 in items)
                {
                    if (item2.Value == bridge.PortA)
                    {
                        x1 = item2.Key.X * landSizeX + (landSizeX / 2);
                        y1 = item2.Key.Y * landSizeY + (landSizeY / 2);
                    }else if (item2.Value == bridge.PortB)
                    {
                        x2 = item2.Key.X * landSizeX + (landSizeX / 2);
                        y2 = item2.Key.Y * landSizeY + (landSizeY / 2);
                    }
                }
                bool isHorizontal = Math.Abs(y1 - y2) < 0.1;
                var lines = new List<Line>();
                switch (bridge.BridgeNumber)
                {
                    case 1:
                        lines.Add(new Line { StartPoint = new Point(x1, y1), EndPoint = new Point(x2, y2), Stroke = bridgeBrush, StrokeThickness = 1 });
                        break;
                    case 2:
                        // 添加两条相互平行的线
                        if (isHorizontal)
                        {
                            // 如果是水平线，则需要增加两条水平线，y坐标不同
                            double offset = 10; // 你可以调整这个偏移量
                            lines.Add(new Line { StartPoint = new Point(x1, y1 - offset), EndPoint = new Point(x2, y2 - offset), Stroke = bridgeBrush, StrokeThickness = 1 });
                            lines.Add(new Line { StartPoint = new Point(x1, y1 + offset), EndPoint = new Point(x2, y2 + offset), Stroke = bridgeBrush, StrokeThickness = 1 });
                        }
                        else
                        {
                            // 如果是竖直线，则需要增加两条竖直线，x坐标不同
                            double offset = 10; // 你可以调整这个偏移量
                            lines.Add(new Line { StartPoint = new Point(x1 - offset, y1), EndPoint = new Point(x2 - offset, y2), Stroke = bridgeBrush, StrokeThickness = 1 });
                            lines.Add(new Line { StartPoint = new Point(x1 + offset, y1), EndPoint = new Point(x2 + offset, y2), Stroke = bridgeBrush, StrokeThickness = 1 });
                        }

                        break;
                    default:
                        break;
                }


                foreach (var line in lines)
                {
                    canvas.Children.Add(line);
                }

            }
        }
        // 绘制岛数字
        foreach (var item in items)
        {
            if (item.Value is Land land)
            {
                //var circle = new Ellipse
                //{
                //    Width = landSizeX,
                //    Height = landSizeY,
                //    Stroke = landBrush,
                //    Fill = landBgBrush,
                //    StrokeThickness = 1
                //};
                //Canvas.SetLeft(circle, item.Key.X * landSizeX);
                //Canvas.SetTop(circle, item.Key.Y * landSizeY);
                //canvas.Children.Add(circle);
                //if (land.Number != 0)
                //{
                //    var text = new TextBlock
                //    {
                //        Text = land.Number.ToString(),
                //        Foreground = textBrush,
                //        FontSize = fontSize,
                //        FontFamily = font
                //    };
                //    Canvas.SetLeft(text, item.Key.X * landSizeX + landSizeX / 2 - fontSize / 2);
                //    Canvas.SetTop(text, item.Key.Y * landSizeY + landSizeY / 2 - fontSize / 2);
                //    canvas.Children.Add(text);
                //}
                var lc = new LandControl()
                {
                    Width = landSizeX,
                    Height = landSizeY,
                    TargetLand = land
                };
                lc.landButton.Background = landBgBrush;
                lc.landButton.BorderBrush = landBrush;
                lc.landButton.BorderThickness = new Thickness(1);
                lc.landButton.Foreground = textBrush;
                lc.landButton.Content = land.Number.ToString();
                lc.ZIndex = 3;
                if (click != null)
                {
                    lc.LandButtonClickEvent += click;
                }
                
                Canvas.SetLeft(lc, item.Key.X * landSizeX);
                Canvas.SetTop(lc, item.Key.Y * landSizeY);
                canvas.Children.Add(lc);

            }
        }
    }
}
