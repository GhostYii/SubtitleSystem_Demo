## Subtitle System介绍
SubtitleSystem是一个便于在游戏内添加字幕以及拓展的Unity开源项目，目前(2020.02.02)已经开发完成第一个版本，其主要功能包括：  
- 创建SubtitleAsset资源以持久化存储字幕
- 使用SubtitleAssetEditorWindow以方便查看与编辑字幕文件
- 使用SubtitleAssetPlayer以实现字幕的顺序播放
- 使用SubtitleManager单例以实现字定义字幕显示
- 使用SubtitleSequence以实现字定义字幕序列
- 使用Subtitle以拓展字幕显示形式

## Subtitle Asset资源的设置
如果需要实现对游戏内的过场CG或者视频添加字幕文件，可以通过SubtitleAsset来对字幕进行管理与播放。**SubtitleAsset**是SubtitleSystem内自定义的一个ScriptableObject资源，可以方便的进行保存与修改，同时，该系统编写了一个专门用于编辑SubtitleAsset的编辑器界面可用于查看与编辑字幕。  
创建SubtitleAsset的过程与创建其他类型的资源类似，可以通过在*Asset窗口右键-Create-Subtitle Asset*实现，如下图所示：  
![create asset](https://github.com/GhostYii/SubtitleSystem_Demo/tree/master/ReadmeImg/createasset.png)  
也可以通过Unity顶部的Asset选项卡创建，*Asset-Create-Subtitle Asset*，如下图所示：  
![create asset by menu](https://github.com/GhostYii/SubtitleSystem_Demo/tree/master/ReadmeImg/createasset1.png)  
创建完成后Unity将生成一个Asset文件，选择该文件将显示该字幕文件的简要信息，  
![brief info](https://github.com/GhostYii/SubtitleSystem_Demo/tree/master/ReadmeImg/subtitleinspector.png)  
在此界面，可以选择“从文件导入”、“导出到文件”、“打开编辑器”三项操作，其中导入导出均对.sa文件进行操作，打开编辑器可以查看本文件的详细字幕内容。编辑器分为四个部分，如下图所示  
![asset editor](https://github.com/GhostYii/SubtitleSystem_Demo/tree/master/ReadmeImg/asseteditor.png)  
其中红色部分代表本字幕文件一些说明信息，包括Version与Info两个字段，黄色部分代表本字幕文件所需要的所有格式信息，蓝色部分代表所有字幕内容。底部功能按钮区可以对本文件进行导入导出操作。值得注意的一点是，**在Formats(黄色区域)内的Font字段所选择的字体，应该全部存放在Resources/Font文件夹下**，否则SubtitleSystem将无法正确寻找到对应的字体而报错。Formats区域所选择的格式操作基本是属于UnityEngine.UI.Text组件的属性。  
Subtitles区域则是整个字幕文件的主要区域，其每个字幕item的前两个属性用于选择应用于此条字幕的格式，如果选择FormatIndex，则使用Index寻找格式，格式的Index在格式名称后使用“**[]**”标注，如果选择FormatCode，则会去寻找Formats区域内第一个Code匹配的格式。同一条字幕只能对应一种格式。Position字段标明该条字幕显示在屏幕上的位置，其对应Text组件的rectTransform.localPosition属性。Content标明字幕内容，Duration标明此条字幕的持续时间。  
其中Subtitles区域内的导入导出功能仅对此区域有效，导入可选择.txt格式或者该系统定义的.subs格式，导出仅可导出.subs格式，其中.txt格式会将txt文件内的每一行当作一条字幕导入，其余属性将使用默认属性，如果需要自定义属性，可以通过以下格式定义：
```
"content"|"FormatIndex"|"FormatCode"|"Duration"|"(position.x, position.y)"
```
其中每个字段使用两个**"**符号包括，在**"**内部可以继续使用**"**符号不会影响导入，每个字段之间使用**|**符号隔开，其中字段顺序如上所述。如果选择FormatIndex来确定格式，则FormatCode保持为空，反之则FormatIndex保持为-1。  
值得注意的是，如果游戏内的格式几乎完全一致，那么可以通过保存一个只存有格式的.sa文件（通过Subtitle Asset Editor的Save to (SA)file功能）与不同的.subs（通过Subtitle Asset Editor内Subtitles区域的Save subtitles to file功能或者按照上述格式手动创建）来动态创建SubtitleAsset，在代码中可以使用以下代码实现。
```csharp
SubtitleAsset CreateAsset()
{
    SubtitleAsset sa = SubtitleAsset.CreateInstance<SubtitleAsset>();
    sa.LoadFromFile("format_templete.sa");
    string[] lines = System.IO.File.ReadAllLines("subtiles.subs");        
    foreach (var line in lines)
    {
        SubtitleInfo tmp = new SubtitleInfo();
        tmp.InitByFormat(line);
        sa.subtitles.Add(tmp);
    }
    return sa;
}
```
## Subtitle Asset Player使用
当SubtitleAsset设置完成后，可以使用SubtitleAssetPlayer脚本来实现播放功能，其使用方法非常简单，**首先在场景中任意物体上挂载一个SubtitleManager脚本并且指定一个CanvasPrefab**，这一步是使用SubtitleSystem的必要条件，在本文所介绍的字幕系统中，所有所有播放操作都需要用到SubtitleManager脚本，**故在进行操作前务必先指定SubtitleManager**。之后在场景任意物体上挂载一个SubtitleAssetPlayer脚本即可。  
该脚本主要需要指定一个SubtitleAsst资源以实现播放，用户可以指定该Asset播放的Text组件，如果Text未指定则会在运行时创建一个动态的Text。同时在该脚本的Inspector面板存在三个控制按钮，分别是Play、Pause、Stop按钮以方便对SubtitleAsset资源的进度控制。在这三个按钮的下方是简易监视面板，此面板在设置完成后，在运行时可以监视当前播放的字幕信息与进度，具体效果如下图所示：
![subtitle asset player](https://github.com/GhostYii/SubtitleSystem_Demo/tree/master/ReadmeImg/assetplayer.gif)


## Subtitle Manager使用
在上文中已经提到SubtitleManager组件是整个字幕系统正确运行的必要条件，该脚本是一个单例脚本，在工程任意位置中可以使用`SubtitleManager.Instance`来获取本脚本的实例，该组件中存在一部分控制代码，其中比较重要的一些公共方法如下：
```csharp
//创建一个动态Text组件
public Text CreateText(Vector3 position, int fontSize, Color color, string fontName = "Arial");

//显示字幕，duration<=0时该字幕会永久显示
//Show存在其他8个重载
public Subtitle Show(string content, float duration = 0, bool playOnCreate = true);

//显示竖排字幕，duration<=0时该字幕会永久显示
//ShowVertical存在其他6个重载
public Subtitle ShowVertical(string content, float duration = 0, bool playOnCreate = true);

//显示淡入淡出字幕
public Subtitle ShowWithFade(string content, Vector3 position, int fontSize, Color color, float duration, float fadeInDuration, float fadeOutDuration, string fontName = "Arial", bool playOnCreate = true);

//显示带震动效果的字幕
public Subtitle ShowWithShakePosition(string content, Vector3 position, int fontSize, Color color, float duration, float intensity, bool fadeOut = false, string fontName = "Arial", bool playOnCreate = true);
public Subtitle ShowWithShakeRotation(string content, Vector3 position, int fontSize, Color color, float duration, float intensity, bool fadeOut = false, string fontName = "Arial", bool playOnCreate = true);
public Subtitle ShowWithShakeScale(string content, Vector3 position, int fontSize, Color color, float duration, float intensity, bool fadeOut = false, string fontName = "Arial", bool playOnCreate = true);

//显示打字机效果字幕
public Subtitle ShowWithTypewriter(string content, Vector3 position, int fontSize, Color color, float duration, float interval, string fontName = "Arial", bool playOnCreate = true);

//显示自定义效果字幕
//onShow(Text text, float currentProgress)
public Subtitle ShowWithCustom(string content, Vector3 position, int fontSize, Color color, float duration, Action<Text, float> onShow, Action onComplete, string fontName = "Arial", bool playOnCreate = true);
```
其各个效果预览如下：  
![subtitle demo](https://github.com/GhostYii/SubtitleSystem_Demo/tree/master/ReadmeImg/subtitledemo.gif)

其中`ShowWithCustom`方法可以对字幕显示效果进行自定义，通过参数`onShow`与`onComplete`对字幕显示过程与停止的行为进行自定义以达到自定义效果。如上图中颜色渐变可以通过以下代码实现：
```csharp
SubtitleManager.Instance.ShowWithCustom("自定义效果字幕，字体颜色渐变为红色", Vector3.zero, 30, Color.white, 2f, (t, d) => t.DOColor(Color.red, 3f));
```

## Subtitle Sequence使用
除了使用SubtitleAsset资源进行固定序列字幕之外，SubtitleSystem也支持使用代码动态生成序列进行播放，在代码中使用到的类为SubtitleSequence，此序列用法类似于DOTween中的Sequence，其主要API如下：
```csharp
//尾部添加
public void Append(Subtitle item);
//尾部添加空白间隔
public void AppendInterval(float interval);
//插入字幕
public void Insert(int index, Subtitle item);
//插入间隔
public void InsertInterval(int index, float interval);
//添加同时播放的字幕
public void Join(Subtitle item);
public void Join(int index, Subtitle item);

public void Start();
public void TogglePause();
public void Stop();
public void Restart();
```
上一部分中各个字幕的演示效果Sequence代码如下：
```csharp
SubtitleSequence sequence = new SubtitleSequence();
sequence.Append(SubtitleManager.Instance.Show("字幕测试，所有字幕显示时间2s", Vector3.zero, 30, Color.white, "Arial", 2f, false));
sequence.Append(SubtitleManager.Instance.ShowVertical("竖排字幕预览", Vector3.zero, 30, Color.white, 2f, false));
sequence.Append(SubtitleManager.Instance.ShowWithFade("淡入淡出字幕，淡入淡出各2s", Vector3.zero, 30, Color.white, 0, 2f, 2f, "Arial", false));
sequence.Append(SubtitleManager.Instance.ShowWithShakePosition("位置震动字幕", Vector3.zero, 30, Color.white, 2f, 30f, true, "Arial", false));
sequence.Append(SubtitleManager.Instance.ShowWithShakeRotation("旋转震动字幕", Vector3.zero, 30, Color.white, 2f, 90f, true, "Arial", false));
sequence.Append(SubtitleManager.Instance.ShowWithShakeScale("缩放震动字幕", Vector3.zero, 30, Color.white, 2f, 5f, true, "Arial", false));
sequence.Append(SubtitleManager.Instance.ShowWithTypewriter("打字机效果预览字幕", Vector3.zero, 30, Color.white, 2f, 0.1f, "Arial", false));
sequence.Append(SubtitleManager.Instance.ShowWithCustom("自定义效果字幕，字体颜色渐变为红色", Vector3.zero, 30, Color.white, 2f, (t, d) => t.DOColor(Color.red, 3f), null, "Arial", false));
sequence.Append(new Subtitle("常驻字幕，永不消失", Vector3.zero, 30, Color.white, "Arial", 0));    

sequence.Play();
```
值得注意的一点是使用SubtitleManager.Instance产生的内置效果字幕默认状态下会在创建后立即播放，如果需要加入Sequence统一管理，则需要将最后一个参数设置为false，如果使用的是Subtitle的构造函数则不需要进行设置。

## 其他API
```csharp
SubtitleUtility.cs
---
//等待duration时间后执行action
public static void WaitSecondsForSomething(Action action, float duration);
//获取字体
public static Font GetFontByFontName(string fontName);
//在非Mono脚本中执行协程
public static void StartCoroutine(IEnumerator routine);
public static void StartCoroutine(IEnumerator routine, string routineTag);
//终止某tag上的所有协程
public static void StopAllCoroutines(string coroutineTag);
```

## 特别感谢
本系统开发过程中使用了以下开源/免费项目：
1. <a href="http://dotween.demigiant.com/" target="_blank">DOTween</a>
2. <a href="https://github.com/aaubry/YamlDotNet" target="_blank">YamlDotNet</a>