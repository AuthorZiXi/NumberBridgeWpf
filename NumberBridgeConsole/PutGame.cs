using NumberBridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberBridgeConsole;

public static class PutGame
{
    public static void Put(NumberBridgeGamePanel panel)
    {
        var outmsg = string.Empty;
        if (panel.Items.Count > 0)
        {
            //for (int xl = 0; xl < panel.PanelWidth; xl++)
            //{
            //    outmsg += "+-+";
            //}
            outmsg += Environment.NewLine;
            for (int j = 0; j < panel.PanelHeight; j++)
            {
                outmsg += $"{j+1} 行";
                for (int i = 0; i < panel.PanelWidth; i++)
                {
                    var pos = new Pos(i, j);
                    outmsg += "【";
                    if (panel.Items.ContainsKey(pos))
                    {
                        var item = panel.Items[pos];
                        if (item is Land land)
                        {
                            var ns = land.Number.ToString();
                            //switch (land.Number)
                            //{
                            //    case 1:
                            //        ns = "①";
                            //        break;
                            //    case 2:
                            //        ns = "②";
                            //        break;
                            //    case 3:
                            //        ns = "③";
                            //        break;
                            //    case 4:
                            //        ns = "④";
                            //        break;
                            //    case 5:
                            //        ns = "⑤";
                            //        break;
                            //        case 6:
                            //        ns = "⑥";
                            //        break;
                            //    case 7:
                            //        ns = "⑦";
                            //        break;
                            //    case 8:
                            //        ns = "⑧";
                            //        break;
                            //    default:
                            //        ns = "#";
                            //        break;
                            //}
                            outmsg += ns;
                        }
                        else if (item is Bridge bridge)
                        {
                            switch (bridge.BridgeNumber)
                            {
                                case 1:
                                    outmsg += "*";
                                    break;
                                case 2:
                                    outmsg += "=";
                                    break;
                                default:
                                    outmsg += "错";
                                    break;
                            }
                            //outmsg += bridge.BridgeNumber.ToString();
                        }
                    }
                    else
                    {
                        outmsg += " ";
                    }
                    outmsg += "】";
                }

                outmsg += Environment.NewLine;
                
                //for (int xl = 0; xl < panel.PanelWidth; xl++)
                //{
                //    outmsg += "+-+";
                //}
                outmsg += Environment.NewLine;
            }
            Console.WriteLine(outmsg);
        }
        else
        {
            Console.WriteLine("还没有数据");
        }
    }
    public static void PutBaseMap(NumberBridgeGamePanel panel,Dictionary<Pos, FillTemplate> baseMap)
    {
        var outmsg = string.Empty;
        var width = panel.PanelWidth;
        var height = panel.PanelHeight;
        //for (int xl = 0; xl < panel.PanelSizeX; xl++)
        //{
        //    outmsg += "+  ";
        //}
        outmsg += Environment.NewLine;
        if (baseMap.Count > 0)
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    var pos = new Pos(i, j);
                    outmsg += "[";
                    if (baseMap.ContainsKey(pos))
                    {
                        var item = baseMap[pos];
                        switch (item)
                        {
                            case FillTemplate.Empty:
                                outmsg += " ";
                                break;
                            case FillTemplate.Land:
                                outmsg += "L";
                                break;
                            case FillTemplate.WillFillBridge:
                                outmsg += "B";
                                break;
                        }
                    }
                    else
                    {
                        outmsg += " ";
                    }
                    outmsg += "]";
                }
                outmsg += Environment.NewLine;
                //for (int xl = 0; xl < panel.PanelSizeX; xl++)
                //{
                //    outmsg += "   ";
                //}
                //outmsg += Environment.NewLine;
            }
            Console.WriteLine(outmsg);
        }
        else
        {
            Console.WriteLine("还没有数据");
        }
        
    }
}
