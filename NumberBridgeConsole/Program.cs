// See https://aka.ms/new-console-template for more information
using NumberBridge;
using NumberBridgeConsole;

Console.WriteLine("Hello, World!");
var gp = new NumberBridgeGamePanel(10, 10);
//gp.CreateNewGame();
//PutGame.Put(gp);
while (true)
{
    var bm = gp.CreateBaseMap();
    PutGame.PutBaseMap(gp, bm);
    gp.ConvertMap(bm);
    PutGame.Put(gp);
    gp.ClearBridges();
    Console.WriteLine("用 * 代表一个桥， = 代表两座桥， 数字表示该岛需要的桥的数量");
    PutGame.Put(gp);
    Console.WriteLine("q to quit");
    var c = Console.ReadLine();
    if (c == "q")
    {
        break;
    }
}
