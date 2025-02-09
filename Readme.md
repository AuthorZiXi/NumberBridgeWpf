# NumberBridgeWpf

“简单的”数桥游戏，用`C#+wpf`制作。

界面预览：

![界面](view.png)

**使用 MIT LICENSE**

下载在 Releases

## 项目组成

- NumberBridge : 核心游戏逻辑，包括地图生成，桥梁连接，相关判断
- NumberbridgeConsole : 地图生成测试，无游戏功能
- NumberBridgeWpf : 游戏程序，交互与地图转换

> 注意: 
>
>PutGame.cs 在 `NumberBridgeWpf` 是将地图数据转换成控件交互的方法类，但 `NumberbridgeConsole` 是地图转换成控制台文本的方法类
## 开发历程与心得

- 一切的开始

24年时，偶然得知数桥谜题，了解游戏规则后，有了开发的写法

- 25.1.27 开始制作

在制作过程期间，多次使用搜索，还是发现没人`用除js以外的语言`做这类游戏。

网上有的基本上也是`html+js`写的，没有开放任何有用的源代码

我仅凭兴趣去开发了这个程序，代码质量不能保证。

- 25.1.28 基础地图生成逻辑

参见 `NumberBridgeGamePanel实例.CreateBaseMap()` 

- 25.1.29 基础地图生成逻辑完成

- 25.1.30 完成并打包Setup

包括正常的谜题地图随机生成，求解，游玩功能



