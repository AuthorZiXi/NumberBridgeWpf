using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NumberBridge;

public struct Pos
{
    public int X { get; set; }
    public int Y { get; set; }
    public Pos(int x, int y)
    {
        X = x;
        Y = y;
    }
    public override string ToString()
    {
        return $"({X},{Y})";
    }
}
public enum Face
{
    Unknown,
    Top,
    Bottom,
    Left,
    Right
}
public static class FaceHelper
{
    public static Face GetOpposite(this Face face)
    {
        return face switch
        {
            Face.Top => Face.Bottom,
            Face.Bottom => Face.Top,
            Face.Left => Face.Right,
            Face.Right => Face.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
        };
    }
}
public static class LandHelper
{
    public static int GetCanLinkBridgeCount(List<Face> canWalkFace)
    {
        var canLinkBridgeCount = 0;
        if (canWalkFace.Contains(Face.Top))
        {
            canLinkBridgeCount+=2;
        }
        if (canWalkFace.Contains(Face.Bottom))
        {
            canLinkBridgeCount+=2;
        }
        if (canWalkFace.Contains(Face.Left))
        {
            canLinkBridgeCount+=2;
        }
        if (canWalkFace.Contains(Face.Right))
        {
            canLinkBridgeCount+=2;
        }
        return canLinkBridgeCount;
    }
}

public abstract class GameItem
{

}
public class Land : GameItem
{
    public int Number { get; set; } = 0;
    public List<Bridge> Bridges { get; set; } = [];
    public bool CanBuildBridge()
    {
        
        return GetBridgeCount() < Number;
    }
    public int GetBridgeCount()
    {
        var usedCount = 0;
        foreach (var bridge in Bridges)
        {
            usedCount += bridge.BridgeNumber;
        }
        return usedCount;
    }
    public int CanBuildBridgeCount()
    {
        return Number - GetBridgeCount();
    }
    public override string ToString()
    {
        return $"Land {Number} {Bridges.Count}";
    }
}
public class Bridge(Land portA, Land portB) : GameItem
{
    public Land PortA { get; set; } = portA;
    public Land PortB { get; set; } = portB;
    // 最大为2
    public int BridgeNumber { get; set; } = 1;

}
