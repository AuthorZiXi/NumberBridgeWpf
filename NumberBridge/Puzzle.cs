namespace NumberBridge;


public enum FillTemplate
{
    Empty,
    Land,
    WillFillBridge,
}

public class NumberBridgeGamePanel
{
    public Dictionary<Pos, GameItem> Items { get; set; } = [];
    private Dictionary<Pos, GameItem> solvedItems { get; set; } = [];
    public int PanelWidth { get; set; } = 6;
    public int PanelHeight { get; set; } = 6;
    public NumberBridgeGamePanel(int panelSizeX, int panelSizeY)
    {
        if (panelSizeX < 6)
        {
            throw new ArgumentException("Panel size must be at least 6");
        }
        if (panelSizeY < 6)
        {
            throw new ArgumentException("Panel size must be at least 6");
        }
        PanelWidth = panelSizeX;
        PanelHeight = panelSizeY;
    }
    public void CreateNewGame()
    {
        var bm = CreateBaseMap();
        ConvertMap(bm);
        ClearBridges();
    }
    public Dictionary<Pos, FillTemplate> CreateBaseMap()
    {
        Items.Clear();
        var random = new Random();
        var panel = new Dictionary<Pos, FillTemplate>();
        var startX = random.Next(0, PanelWidth);
        var startY = random.Next(0, PanelHeight);
        var startPos = new Pos(startX, startY);
        var panelSizeWith = PanelWidth;
        var panelSizeHeight = PanelHeight;
        
        
        void WalkLand(Pos pos,Face? parentWhereToYou=null)
        {
            var canWalkPos = new Dictionary<Pos, Face>();
            var canWalkFace = new List<Face>();
            var walkingPos = pos;
            var nowTryWalkFace = Face.Top;
            var mustSpace = 1;
            var walkMustSpaceCount = 0;
            void ResearchCanWalkPos()
            {
                canWalkPos.Clear();
                canWalkFace.Clear();
                nowTryWalkFace = Face.Top;
                walkingPos = pos;
                walkMustSpaceCount = 0;
                // 收集可以步进的坐标及朝向
                while (true)
                {
                    switch (nowTryWalkFace)
                    {
                        case Face.Top:
                            walkingPos = new Pos(walkingPos.X, walkingPos.Y - 1);
                            break;
                        case Face.Bottom:
                            walkingPos = new Pos(walkingPos.X, walkingPos.Y + 1);
                            break;
                        case Face.Left:
                            walkingPos = new Pos(walkingPos.X - 1, walkingPos.Y);
                            break;
                        case Face.Right:
                            walkingPos = new Pos(walkingPos.X + 1, walkingPos.Y);
                            break;
                    }

                    //终止步进此方向
                    if (walkingPos.Y < 0 || walkingPos.Y >= panelSizeHeight || walkingPos.X < 0 || walkingPos.X >= panelSizeWith || panel.ContainsKey(walkingPos))
                    {
                        var isEnd = false;
                        switch (nowTryWalkFace)
                        {
                            case Face.Top:
                                nowTryWalkFace = Face.Bottom;
                                break;
                            case Face.Bottom:
                                nowTryWalkFace = Face.Left;
                                break;
                            case Face.Left:
                                nowTryWalkFace = Face.Right;
                                break;
                            case Face.Right:
                                isEnd = true;
                                break;
                        }
                        if (isEnd) break;
                        walkingPos = pos;
                        walkMustSpaceCount = 0;
                    }
                    else
                    {
                        if (walkMustSpaceCount < mustSpace)
                        {
                            walkMustSpaceCount++;
                            continue;
                        }
                        canWalkPos.Add(walkingPos, nowTryWalkFace);
                        if (!canWalkFace.Contains(nowTryWalkFace))
                        {
                            canWalkFace.Add(nowTryWalkFace);
                        }
                    }
                }
            }
            
            ResearchCanWalkPos();
            
            if (parentWhereToYou != null)
            {
                canWalkFace.Remove(parentWhereToYou.Value);
                var newCanWalkPos = new Dictionary<Pos, Face>();
                foreach (var item in canWalkPos)
                {
                    if (item.Value != parentWhereToYou.Value)
                    {
                        newCanWalkPos.Add(item.Key, item.Value);
                    }
                }
                canWalkPos = newCanWalkPos;
            }
            if (canWalkFace.Count == 0)
            {
                return;
            }
            else
            {
                //多个朝向处理
                var spawnLandCount = 0;
                var decideLoopCount = 0;
                if (canWalkFace.Count > 1)
                {
                    //Console.WriteLine($"『 可步进方向有多个 {string.Join(",", canWalkFace)} 』");
                    var decideSubWalkFace = random.Next(1, canWalkFace.Count + 1);
                    if (decideSubWalkFace != 1)
                    {
                        decideLoopCount = decideSubWalkFace;
                        //Console.WriteLine($"『 可步进方向有多个随机计划 {decideSubWalkFace} 个』");
                    }
                    //else
                    //{
                    //    //Console.WriteLine($"『 可步进方向有多个可是没有随机计划多个』");
                    //}
                }
                
                var decideWalkFace = canWalkFace[random.Next(0, canWalkFace.Count)];
                var decideCanWalkPos = new List<Pos>();
                spawnnewsubland:
                if (canWalkFace.Count == 0) return;
                decideWalkFace = canWalkFace[random.Next(0, canWalkFace.Count)];
                decideCanWalkPos.Clear();
                foreach (var item in canWalkPos)
                {
                    if (item.Value == decideWalkFace)
                    {
                        decideCanWalkPos.Add(item.Key);
                    }
                }
                if (decideCanWalkPos.Count == 0) throw new Exception("不符合事实的decideCanWalkPos.Count == 0，在可步进方向中不可能找不到可步进的坐标");
                var decideWalkPos = decideCanWalkPos[random.Next(0, decideCanWalkPos.Count)];
                // 移除已经走过的方向
                canWalkFace.Remove(decideWalkFace);
                foreach (var item in decideCanWalkPos)
                {
                    canWalkPos.Remove(item);
                }

                spawnLandCount++;
                if (panel.ContainsKey(decideWalkPos))
                {
                    goto spawnnewsubland;
                }
                
                //Console.WriteLine($"『 决定走向 {decideWalkPos} 决定朝向 {decideWalkFace}"+Environment.NewLine+ $"   来自 {pos} 它在此岛的 {parentWhereToYou} 』");
                // 加上它自己
                panel.Add(decideWalkPos, FillTemplate.Land);
                // 如果父母没加加上
                panel.TryAdd(pos, FillTemplate.Land);
                // 在自己与父母之间用桥填上
                if (decideWalkPos.X == pos.X)
                {
                    for (int i = Math.Min(decideWalkPos.Y, pos.Y) + 1; i < Math.Max(decideWalkPos.Y, pos.Y); i++)
                    {
                        var bpos = new Pos(decideWalkPos.X, i);
                        if (panel.ContainsKey(bpos)) throw new Exception("不符合事实的岛屿之间建桥不能交叉");
                        panel.Add(bpos, FillTemplate.WillFillBridge);
                        canWalkPos.Remove(bpos);
                    }
                }
                else if (decideWalkPos.Y == pos.Y)
                {
                    for (int i = Math.Min(decideWalkPos.X, pos.X) + 1; i < Math.Max(decideWalkPos.X, pos.X); i++)
                    {
                        var bpos = new Pos(i, decideWalkPos.Y);
                        if (panel.ContainsKey(bpos)) throw new Exception("不符合事实的岛屿之间建桥不能交叉");
                        panel.Add(bpos, FillTemplate.WillFillBridge);
                        canWalkPos.Remove(bpos);
                    }
                }
                else
                {
                    throw new Exception("不符合事实的岛屿之间建桥只能横向或纵向");
                }
                
                // 子孙处理
                WalkLand(decideWalkPos,FaceHelper.GetOpposite(decideWalkFace));

                // 如果还有剩余的朝向，继续递归
                if (spawnLandCount < decideLoopCount)
                {
                    ResearchCanWalkPos();
                    goto spawnnewsubland;
                }
                



            }
        }
        WalkLand(startPos);
        return panel;
    }
    public void ConvertMap(Dictionary<Pos, FillTemplate> baseMap)
    {
        var items = new Dictionary<Pos, GameItem>();
        var walkedPos = new List<Pos>();
        var random = new Random();
        foreach (var pos in baseMap.Keys)
        {
            if (baseMap[pos] == FillTemplate.WillFillBridge)
            {
                continue;
            }
            else if (baseMap[pos] == FillTemplate.Land)
            {
                var sourceLand = baseMap[pos];
                var faceHasLands = new Dictionary<Face,Pos>();
                var faceHasBridges = new Dictionary<Pos,Face>();
                var walkingPos = pos;
                var nowTryWalkFace = Face.Top;
                var panelSizeWith = PanelWidth;
                var panelSizeHeight = PanelHeight;
                var isEnd = false;
                var isEndTheFaceSearch = false;
                //Console.WriteLine($"=> {pos}");
                // 搜索
                while (true)
                {
                    if (!isEnd)
                    {
                        switch (nowTryWalkFace)
                        {
                            case Face.Top:
                                walkingPos = new Pos(walkingPos.X, walkingPos.Y - 1);
                                break;
                            case Face.Bottom:
                                walkingPos = new Pos(walkingPos.X, walkingPos.Y + 1);
                                break;
                            case Face.Left:
                                walkingPos = new Pos(walkingPos.X - 1, walkingPos.Y);
                                break;
                            case Face.Right:
                                walkingPos = new Pos(walkingPos.X + 1, walkingPos.Y);
                                break;
                        }
                    }
                    
                    var outbound = walkingPos.Y < 0 || walkingPos.Y >= panelSizeHeight || walkingPos.X < 0 || walkingPos.X >= panelSizeWith;
                    var isNotExistOrWalked = !baseMap.ContainsKey(walkingPos) || walkedPos.Contains(walkingPos);
                    //Console.WriteLine($"walkingPos:{walkingPos} outbound:{outbound} isEnd:{isEnd} isEndTheFaceSearch:{isEndTheFaceSearch} isNotExistOrWalked:{isNotExistOrWalked} nowTryWalkFace:{nowTryWalkFace}");
                    if (outbound
                        || isNotExistOrWalked
                        || isEndTheFaceSearch
                        || isEnd )
                    {
                        
                        switch (nowTryWalkFace)
                        {
                            case Face.Top:
                                nowTryWalkFace = Face.Bottom;
                                break;
                            case Face.Bottom:
                                nowTryWalkFace = Face.Left;
                                break;
                            case Face.Left:
                                nowTryWalkFace = Face.Right;
                                break;
                            case Face.Right:
                                isEnd = true;
                                break;
                        }
                        isEndTheFaceSearch = false;
                        walkingPos = pos;
                        if (isEnd) break;
                    }
                    else
                    {
                        switch (baseMap[walkingPos])
                        {
                            case FillTemplate.Land:
                                if (faceHasBridges.ContainsValue(nowTryWalkFace))
                                    faceHasLands.Add(nowTryWalkFace, walkingPos);
                                isEndTheFaceSearch = true;
                                break;
                            case FillTemplate.WillFillBridge:
                                faceHasBridges.Add(walkingPos, nowTryWalkFace);
                                break;
                            default:
                                isEnd = true;
                                break;
                        }


                    }
                }
                foreach (var face in faceHasLands.Keys)
                {
                    var endLandPos = faceHasLands[face];
                    var needFillBridgePos = new List<Pos>();
                    foreach (var bridges in faceHasBridges)
                    {
                        if (bridges.Value == face)
                        {
                            needFillBridgePos.Add(bridges.Key);
                        }
                    }
                    if (needFillBridgePos.Count == 0) throw new Exception("没有找到需要填充的桥坐标");
                    var bridgeLinkCount = random.Next(1,3);// 随机生成1-2个桥，至于为什么写3因为random的第二个参数值是取不到的
                    var sl = new Land();
                    if (items.TryGetValue(pos, out GameItem? svalue))
                    {
                        sl = svalue as Land;
                        if (sl == null) throw new Exception("没有找到起点岛");
                    }
                    sl.Number += bridgeLinkCount;
                    items.TryAdd(pos,sl);
                    var el = new Land();
                    if (items.TryGetValue(endLandPos, out GameItem? evalue))
                    {
                        el = evalue as Land;
                        if (el == null) throw new Exception("没有找到终点岛");
                    }
                    el.Number += bridgeLinkCount;
                    items.TryAdd(endLandPos, el);
                    var nbridge = new Bridge(sl,el);
                    // 这里新桥的BN默认值为1，所以需要-1
                    nbridge.BridgeNumber += bridgeLinkCount - 1;
                    if (nbridge.BridgeNumber >= 2)
                    {

                    }
                    sl.Bridges.Add(nbridge);
                    el.Bridges.Add(nbridge);
                    foreach (var bridgePos in needFillBridgePos)
                    {
                        if (items.ContainsKey(bridgePos)) throw new Exception("这个位置怎么被占用了？");
                        items.Add(bridgePos, nbridge);
                        walkedPos.Add(bridgePos);
                    }

                }
            }
        }
        Items = items;
        solvedItems = items.AsReadOnly().ToDictionary();
    }
    public void SolveGame()
    {
        Items = solvedItems;
    }
    public bool LinkBridge(Land startLand,Land endLand,out bool needRemove)
    {
        needRemove = false;
        var posAt = GetLandPos(startLand);
        var posBt = GetLandPos(endLand);
        if (posAt == null || posBt == null) return false;
        var posA = posAt.Value;
        var posB = posBt.Value;
        if (!startLand.CanBuildBridge() || !endLand.CanBuildBridge())
        {
            needRemove = true;
            return false;
        }
        var willFillBridgePos = new List<Pos>();
        if (posA.X == posB.X)
        {
            var yMin = Math.Min(posA.Y, posB.Y);
            var yMax = Math.Max(posA.Y, posB.Y);
            for (int i = yMin + 1; i < yMax; i++)
            {
                var pos = new Pos(posA.X, i);
                if (Items.ContainsKey(pos))
                {
                    if (Items[pos] is Bridge bridge)
                    {
                        if ((bridge.PortA == startLand && bridge.PortB == endLand) ||
                            (bridge.PortA == endLand && bridge.PortB == startLand))
                        {
                            if (bridge.BridgeNumber == 1)
                            {
                                bridge.BridgeNumber++;
                                return true;
                            }
                            else
                            {
                                needRemove = true;
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                willFillBridgePos.Add(pos);
            }
        }
        else if (posA.Y == posB.Y)
        {
            var xMin = Math.Min(posA.X, posB.X);
            var xMax = Math.Max(posA.X, posB.X);
            for (int i = xMin + 1; i < xMax; i++)
            {
                var pos = new Pos(i, posA.Y);
                if (Items.ContainsKey(pos))
                {
                    if (Items[pos] is Bridge bridge)
                    {
                        if ((bridge.PortA == startLand && bridge.PortB == endLand) ||
                            (bridge.PortA == endLand && bridge.PortB == startLand))
                        {
                            if (bridge.BridgeNumber == 1)
                            {
                                bridge.BridgeNumber++;
                                return true;
                            }
                            else
                            {
                                needRemove = true;
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                willFillBridgePos.Add(pos);
            }
        }
        else
        {
            return false;
        }
        if (willFillBridgePos.Count == 0) return false;
        var newBridge = new Bridge(startLand, endLand);
        startLand.Bridges.Add(newBridge);
        endLand.Bridges.Add(newBridge);
        foreach (var item in willFillBridgePos)
        {
            Items.Add(item, newBridge);
        }
        return true;
    }
    public bool RemoveBridge(Land startLand, Land endLand)
    {
        var posAt = GetLandPos(startLand);
        var posBt = GetLandPos(endLand);
        if (posAt == null || posBt == null) return false;
        var posA = posAt.Value;
        var posB = posBt.Value;
        var willRemoveBridgePos = new List<Pos>();
        var willRemoveBridge = new List<Bridge>();
        if (posA.X == posB.X)
        {
            var yMin = Math.Min(posA.Y, posB.Y);
            var yMax = Math.Max(posA.Y, posB.Y);
            for (int i = yMin + 1; i < yMax; i++)
            {
                var pos = new Pos(posA.X, i);
                if (Items.ContainsKey(pos))
                {
                    if (Items[pos] is Bridge bridge)
                    {
                        if ((bridge.PortA == startLand && bridge.PortB == endLand) ||
                            (bridge.PortA == endLand && bridge.PortB == startLand))
                        {
                            willRemoveBridgePos.Add(pos);
                            if (!willRemoveBridge.Contains(bridge))
                            {
                                willRemoveBridge.Add(bridge);
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        else if (posA.Y == posB.Y)
        {
            var xMin = Math.Min(posA.X, posB.X);
            var xMax = Math.Max(posA.X, posB.X);
            for (int i = xMin + 1; i < xMax; i++)
            {
                var pos = new Pos(i, posA.Y);
                if (Items.ContainsKey(pos))
                {
                    if (Items[pos] is Bridge bridge)
                    {
                        if ((bridge.PortA == startLand && bridge.PortB == endLand) ||
                            (bridge.PortA == endLand && bridge.PortB == startLand))
                        {
                            willRemoveBridgePos.Add(pos);
                            if (!willRemoveBridge.Contains(bridge))
                            {
                                willRemoveBridge.Add(bridge);
                            }
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }

        if (willRemoveBridgePos.Count == 0) return false;
        if (willRemoveBridge.Count == 0) return false;
        foreach (var item in willRemoveBridgePos)
        {
            Items.Remove(item);
        }
        foreach (var item in willRemoveBridge)
        {
            startLand.Bridges.Remove(item);
            endLand.Bridges.Remove(item);
        }
        return true;
    }
    public bool IsSolved()
    {
        if (Items.Count == 0) return false;
        foreach (var item in Items)
        {
            if (item.Value is Land land)
            {
                if (land.CanBuildBridgeCount() != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public void ClearBridges()
    {
        foreach (var item in Items)
        {
            if (item.Value is Bridge bridge)
            {
                Items.Remove(item.Key);
                bridge.PortA.Bridges.Remove(bridge);
                bridge.PortB.Bridges.Remove(bridge);
            }
        }
    }
    public Pos? GetLandPos(Land land)
    {
        foreach (var item in Items)
        {
            if (item.Value == land)
            {
                return item.Key;
            }
        }
        return null;
    }
    public void GetProgress(out int nowLandComplete, out int landTotalCount)
    {
        nowLandComplete = 0;
        landTotalCount = 0;
        foreach (var item in Items)
        {
            if (item.Value is Land land)
            {
                landTotalCount++;
                if (land.CanBuildBridgeCount() == 0)
                {
                    nowLandComplete++;
                }
            }
        }
    }
}