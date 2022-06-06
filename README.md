# NDITable SDK使用文档 0.2.4

<!-- @import "[TOC]" {cmd="toc" depthFrom=1 depthTo=6 orderedList=false} -->

<!-- code_chunk_output -->

- [NDITable SDK使用文档 0.2.4](#nditable-sdk使用文档-024)
  - [简介](#简介)
    - [基本概念](#基本概念)
  - [环境要求](#环境要求)
  - [在Unity项目中导入SDK](#在unity项目中导入sdk)
  - [快速开始](#快速开始)
    - [Marker的使用](#marker的使用)
    - [校正方案的生成和使用](#校正方案的生成和使用)
    - [通信模块的使用](#通信模块的使用)
    - [综合实践](#综合实践)
      - [源码链接](#源码链接)
      - [单工程项目开发流程](#单工程项目开发流程)
      - [双工程项目开发流程](#双工程项目开发流程)
        - [展示屏开发流程](#展示屏开发流程)
        - [交互屏开发流程](#交互屏开发流程)
      - [打包](#打包)
  - [SDK资源介绍](#sdk资源介绍)
    - [基础](#基础)
      - [Markers](#markers)
      - [NDITableClient](#nditableclient)
      - [NDITableEventSystem](#nditableeventsystem)
      - [校正方案](#校正方案)
      - [NDITableGO](#nditablego)
    - [通信部分](#通信部分)
      - [SkyfallServer](#skyfallserver)
      - [SkyfallClient](#skyfallclient)
      - [FreeCall](#freecall)
      - [调用FreeCall](#调用freecall)

<!-- /code_chunk_output -->
<style>
    img[src$="centerme"] {
    display:block;
    margin: 0 auto;
    }
</style>

## 简介

维度感知桌面开发工具包（NDITable SDK）用于开发**维度感知桌面**内容应用。应用的开发形式支持两种：

+ **双工程项目开发**：这种形式的开发分为**展示屏工程**和**交互屏工程**开发，其中交互屏工程开发负责对接交互功能，并使用内置的通信模块控制展示屏工程。
+ **单工程项目开发**：这种形式的开发将所有功能都放到一个工程下，通过创建两个窗口来分别实现展示屏和交互屏，因此可以直接控制展示屏内容而无需使用内置通信模块。

### 基本概念

<img src="document/images/1-6.png?style=centerme" />


>**展示屏工程**：负责展示三维环境内容，提供供下屏调用的接口（FreeCall)。
>**交互屏工程**：负责用户进行交互通过通信模块发送请求（Request）传给展示屏。
>**校正方案**：用于实现交互屏二维坐标与展示屏三维坐标的转换。
>**Marker**：用户用于操作的交互型输入设备（多个）。

---

## 环境要求

此SDK需要使用**Unity3D 2018.4或更高版本**进行开发。由于用到了UnityPackageManager以及`.Net 4.x`相关Api，所以在使用本SDK时需要使用**Unity2018.4**或更高版本，并且**PlayerSettings/Other Settings**中的API兼容（Api Compatibility Level）要调整至`.Net 4.x`，如图：

<img src="document/images/1-5.png?style=centerme" />

---

## 在Unity项目中导入SDK

### 方法一：直接下载 { ignore : true }

我们已经将SDK包发布在了Gitee与Github上，您可以自行前往下载：

相关链接：

>https://gitee.com/ndengine-studio/nditable-sdk.git
>https://github.com/NDEngine/NDITable.git

### 方法二：通过UnityPackageManager加载SDK { ignore : true }

将以下任何一行复制到您工程的**Packages\manifest.json**中：

 > "com.ndengine.nditable": "https://gitee.com/ndengine-studio/nditable-sdk.git#sdk-0.2.4-rc1",

 > "com.ndengine.nditable": "https://github.com/NDEngine/NDITable.git#sdk-0.2.4-rc1",

<img src="document/images/1-1.png?style=centerme" />

导入成功后，在Unity的`Window/NDITable`下点击导入开发资源，如果需要查看相关试例可以点击导入开发试例。

<img src="document/images/1-2.png?style=centerme" />

> **注意**：点击导入开发试例时会同时导入开发资源

---

## 快速开始

**以下相关试例源码均可在导入开发试例后查看。**

### Marker的使用

> **目的**：本示例用于了解Marker的相关事件和使用。（关于Marker的详细介绍请查看[Markers](#markers)）

#### 导入SDK { ignore = true}

按照[在Unity项目中导入SDK](#在unity项目中导入sdk)所述进行操作。

#### 添加SDK组件 { ignore = true }

**目的**： 使项目可以调用Marker和触屏相关事件。

在**NDITableAssets/Prefabs**中将[NDITableClient](#nditableclient)拖拽到场景中，并将Unity创建UI时自动生成的EventSystem替换为[NDITableEventSystem](#nditableeventsystem)，如图：

<img src="document/images/2-3.png?style=centerme" />

#### 添加Marker { ignore = true }

把`NDITableAssets/prefabs/Markers/NormalMarker`拖入场景，如图：

<img src="document/images/2-4.png?style=centerme" />

#### 添加监听Marker自身的事件 { ignore = true }

创建脚本使Marker在调用各个方法时打出Log，如图：

具体代码如下：
```csharp
using UnityEngine;
using xFrame.NDITable.Unity;
using xFrame.NDITable.Unity.EventSystems;

public class MarkerEvents : MarkerController
{
    public override void MarkerAppear(MarkerEventData data) {
        //Marker放下
        Debug.Log("Marker Appear");
    }

    public override void MarkerUpdate(MarkerEventData data) {
        //Marker刷新
        Debug.Log("Marker Update");
    }

    public override void MarkerDisappear(MarkerEventData data) {
        //Marker拿起
        Debug.Log("Marker Disappear");
    }
}

```

将自定义脚本添加至Marker事件，如图：

<img src="document/images/2-5.png?style=centerme" height = 345/>

**运行并测试：**

运行后按下F1通过鼠标模拟Marker进行测试（方向键左右调整MarkerID，鼠标左键放下，右键拿起，滚轮旋转）：

<img src="document/images/2-6.png?style=centerme" />

#### 添加监听Marker进出UI的事件 { ignore = true }

在Canvas中添加一个UI，当Marker进出它时响应事件。

<img src="document/images/2-7.png?style=centerme" />

在UI上添加NDITableEventTrigger：

<img src="document/images/2-8.png?style=centerme" height = 265/>

创建脚本，具体内容如下：
```csharp
using UnityEngine;
using UnityEngine.UI;

public class ImageOneEventTrigger : MonoBehaviour {

    public Text imageText;

    public void MarkerEnter(MarkerEventData data) {
        GetComponent<Image>().color = Color.green;
        imageText.text = "Marker Enter";
    }

    public void MarkerHover(MarkerEventData data) {
        imageText.text = "Marker Hover";
    }

    public void MarkerExit(MarkerEventData data) {
        GetComponent<Image>().color = Color.red;
        imageText.text = "Marker Exit";
    }
}

```

把写好的脚本挂在UI上，在`NDITableEventTrigger`中添加相关事件。
<img src="document/images/2-9.png?style=centerme" />

**运行并测试：**

运行后按下F1通过鼠标模拟Marker进行测试。
<center>
<img src="document/images/2-10.png" />
<img src="document/images/2-11.png" />
</center>

---

### 校正方案的生成和使用

> **目的**：本示例用于了解校正方案的生成与使用。（关于校正方案的详细介绍请查看[校正方案](#校正方案)）

#### 导入SDK { ignore = true }

按照[在Unity项目中导入SDK](#在unity项目中导入sdk)所述进行操作。

#### 添加SDK组件 { ignore = true }

**目的**： 使项目可以调用Marker和触屏相关事件。

在**NDITableAssets/Prefabs**中将[NDITableClient](#nditableclient)拖拽到场景中，并将Unity创建UI时自动生成的EventSystem替换为[NDITableEventSystem](#nditableeventsystem)，如图：

<img src="document/images/2-3.png?style=centerme" />

#### 添加Marker { ignore = true }

把`NDITableAssets/prefabs/Markers/NormalMarker`拖入场景，如图：

<img src="document/images/2-4.png?style=centerme" />

#### 创建展示屏窗口和交互屏窗口 { ignore = true }

##### 新建UI相机并设置为交互屏窗口 { ignore = true }

在场景中新建一个相机用于拍摄Canvas窗口，并将该相机的`Target Display`设置为Display1作为**交互屏窗口**，如图：

<img src="document/images/4-2.png?style=centerme" />

> **注意**：Marker的显示相机（DisplayCamera）要选为此UI相机：
> <img src="document/images/4-5.png?style=centerme" />

##### 设置3D场景相机为展示屏窗口 { ignore = true }

把用来显示3D场景的原Camera的`Target Display`设置为Display2作为**展示屏窗口**，如图：

<img src="document/images/4-4.png?style=centerme" width = 332 height = 505/>

##### 添加同时显示两个窗口的脚本 { ignore = true }

新建脚本`ShowDisplay.cs`并添加到场景中

ShowDisplay.cs代码如下：
```csharp
using UnityEngine;

public class ShowDisplay : MonoBehaviour
{
    void Start() {
        //显示两个窗口
        for (int i = 0; i < Display.displays.Length; i++) {
            Display.displays[i].Activate();
        }
    }
}
```
> **注意**：把显示两个窗口的脚本写在Unity的Start声明周期中防止初始化失败。


#### 添加校正方案 { ignore = true }

**在`Hierarchy`面板右键菜单中的`2D Object/NDITable`和`3D Object/NDITable`中创建2D和3D锚点，拖动每个锚点中的三个点到对应的相同位置**

<img src="document/images/4-7.png?style=centerme"/>
<img src="document/images/4-6.png?style=centerme"/>

#### 注册校正方案 { ignore = true }

创建脚本并挂在场景中注册生成校正方案。(脚本代码请查看[生成校正方案](#生成校正方案))

#### 通过校正方案转换坐标以完成通过Marker控制展示屏方块 { ignore = true }

具体转换坐标的代码如下：
```csharp
    //将Marker的二维坐标转换为Cube的三维坐标
    var pos = CoordMappingSchemeManager.Instance.Mapping(
        new Vector3((float)(data.Data.X / Screen.width), (1.0f - (float)(data.Data.Y / Screen.height))));

    //设置Cube位置
    Cube.transform.position = new Vector3(pos.x, Cube.transform.position.y, pos.z);

```

#### 运行并测试 { ignore = true }

运行后在交互屏窗口按下F1通过鼠标模拟Marker进行测试，移动Marker到不同颜色的UI上，如果展示屏方块也在相同颜色的地板上则证明测试成功。

<img src="document/images/4-8.png?style=centerme"/>
<img src="document/images/4-9.png?style=centerme"/>

---

### 通信模块的使用

> **目的**：本示例用于了解FreeCall的创建和使用。（关于通信部分的详细介绍请查看[通信部分](#通信部分)）

#### 导入SDK { ignore = true}

按照[在Unity项目中导入SDK](#在unity项目中导入sdk)所述进行操作。

#### 添加SDK组件 { ignore = true }

**目的**： NDITable相关组件负责调用Marker和触屏相关事件，Skyfall相关组件负责开启和连接服务。

在**NDITableAssets/Prefabs**中将[NDITableClient](#nditableclient)、[SkyfallClient](#skyfallclient)、[SkyfallServer](#skyfallclient)拖拽到场景中，并将Unity创建UI时自动生成的EventSystem替换为[NDITableEventSystem](#nditableeventsystem)，如图：

<img src="document/images/5-5.png?style=centerme" />

> **注意**：FreeCall通常使用在双工程项目中，它的作用是让两个工程可以通过通信进行交互，这里在同一个工程中向自己进行通信只是为了展示如何使用FreeCall进行通信。如果您的项目是基于**TUIO协议**开发的，则不用替换[NDITableEventSystem](#nditableeventsystem)，只导入[NDITableClient](#nditableclient)即可。

#### 创建场景 { ignore = true }

我们在场景中放一个红色的正方体，然后在UI上放一个按钮，通过点击按钮发送Request改变正方体的颜色。

<img src="document/images/5-6.png?style=centerme"/>

#### 创建FreeCall { ignore = true }

创建改变正方体颜色的接口供交互屏调用（FreeCall的详细生成请查看[FreeCall](#freecall)）。创建完成后将FreeCall挂在场景中：

<img src="document/images/5-7.png?style=centerme" />

#### 在按钮事件中进行调用 { ignore = true }

在按钮的点击事件中发送Request调用FreeCall方法改变正方体的颜色：

<img src="document/images/5-8.png?style=centerme" />

#### 运行并测试 { ignore = true }
运行工程并点击按钮，如果正方体变为绿色则说明测试成功。

<img src="document/images/5-9.png?style=centerme"  height = 326/>

---

### 综合实践

#### 源码链接

示例的完整代码地址如下，如有需要可自行下载：
https://github.com/NDEngine/NDITable-City.git

---

#### 单工程项目开发流程

单工程开发就是把**展示屏工程**和**交互屏工程**放在同一个场景中进行开发，并用两个窗口(Display)进行展示。使用单个工程开发的优点是可以直接对方法进行调用，不需要使用通信模块进行调用。

##### 新建工程 { ignore = true }

创建Unity项目并根据[环境要求](#环境要求)修改配置。

##### 导入SDK { ignore = true }

按照[在Unity项目中导入SDK](#在unity项目中导入sdk)所述进行操作。

##### 添加SDK组件 { ignore = true }

**目的**： 使项目可以调用Marker和触屏相关事件。

在**NDITableAssets/Prefabs**中将[NDITableClient](#nditableclient)拖拽到场景中，并将Unity创建UI时自动生成的EventSystem替换为[NDITableEventSystem](#nditableeventsystem)，如图：

<img src="document/images/4-1.png?style=centerme" />

##### 创建展示屏窗口和交互屏窗口 { ignore = true }

##### 新建UI相机并设置为交互屏窗口 { ignore = true }

在场景中新建一个相机用于拍摄Canvas窗口，并将该相机的`Target Display`设置为Display1作为**交互屏窗口**，如图：

<img src="document/images/4-2.png?style=centerme" />

##### 设置3D场景相机为展示屏窗口 { ignore = true }

把用来显示3D场景的原Camera的`Target Display`设置为Display2作为**展示屏窗口**，如图：

<img src="document/images/4-4.png?style=centerme" width = 332 height = 505/>

##### 添加同时显示两个窗口的脚本 { ignore = true }

新建脚本`ShowDisplay.cs`并添加到场景中

ShowDisplay.cs代码如下：
```csharp
using UnityEngine;

public class ShowDisplay : MonoBehaviour
{
    void Start() {
        //显示两个窗口
        for (int i = 0; i < Display.displays.Length; i++) {
            Display.displays[i].Activate();
        }
    }
}
```
> **注意**：把显示两个窗口的脚本写在Unity的Start声明周期中防止初始化失败。

#### 添加校正方案 { ignore = true }

**创建锚点**

**目的**：生成[校正方案](#校正方案)

在`Tools/xFrame/NDITable`中创建2D和3D锚点，拖动每个锚点中的三个点到对应的相同位置

<img src="document/images/5-1.png?style=centerme" />
<img src="document/images/5-2.png?style=centerme" width = 584 height = 314/>
<img src="document/images/5-3.png?style=centerme" width = 584 height = 314/>

**注册校正方案**

创建AnchorRegister输入如下代码，并添加到场景：

```csharp
using UnityEngine;
using xFrame.NDITable.Unity;

public class AnchorRegister : MonoBehaviour
{
    void Start()
    {
        string bottomAnchorName = Display.displays[0].renderingWidth > 1300 ? "TouchCity" : "CityBottom";

        var anchorCityBottom = AlignAnchorGroupTable.Instance.GetComponent(bottomAnchorName);
        var anchorCity = AlignAnchorGroupTable.Instance.GetComponent("City");

        var scheme = new CoordMappingScheme(new Vector4[] {
            GetAnchor(anchorCityBottom.Index1.GetComponent<RectTransform>().anchoredPosition),
            GetAnchor(anchorCityBottom.Index2.GetComponent<RectTransform>().anchoredPosition),
            GetAnchor(anchorCityBottom.Index3.GetComponent<RectTransform>().anchoredPosition)
        }, new Vector4[] {
            anchorCity.Index1.transform.position,
            anchorCity.Index2.transform.position,
            anchorCity.Index3.transform.position,
        });

        CoordMappingSchemeManager.Instance.Register("City", scheme);
    }

    private static Vector3 GetAnchor(Vector3 source) {
        return new Vector3(Mathf.Abs(source.x / Screen.width), Mathf.Abs(source.y / Screen.height), 1.0f);
    }
}

```

#### 添加漫游Marker { ignore = true }

将SDK中的FirstPersonMarker拖入场景（[Markers](#markers)）

<img src="document/images/2-2.png?style=centerme" width = 350 />

**新建CameraController**

**目的**：为Marker添加项目的具体实现。

CameraController代码如下：
```csharp
using UnityEngine;
using xFrame.NDITable.Unity;
using xFrame.NDITable.Unity.EventSystems;

public class CameraController : MarkerController {
    //获取漫游相机
    public Camera Controller;

    public override void MarkerAppear(MarkerEventData data) {
        //坐标转换
        var pos = CoordMappingSchemeManager.Instance.Mapping(
            new Vector3((float)(data.Data.X / Screen.width), (1.0f - (float)(data.Data.Y / Screen.height))));
        
        SetCameraPositionAndDirection(pos, (float) data.Data.Angle + GetComponent<Marker>().AngleOffset);
    }

    public override void MarkerUpdate(MarkerEventData data) {
        //坐标转换
        var pos = CoordMappingSchemeManager.Instance.Mapping(
            new Vector3((float)(data.Data.X / Screen.width), (1.0f - (float)(data.Data.Y / Screen.height))));

        SetCameraPositionAndDirection(pos, (float)data.Data.Angle + GetComponent<Marker>().AngleOffset);
    }

    public override void MarkerDisappear(MarkerEventData data) {

    }
```
```csharp
    /// <summary>
    /// 设置漫游相机位置和旋转
    /// </summary>
    /// <param name="position">位置坐标</param>
    /// <param name="angle">旋转角度</param>
    public void SetCameraPositionAndDirection(Vector3 position, float angle) {
        var cam = Controller;

        cam.transform.position = new Vector3(position.x, cam.transform.position.y, position.z);

        cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x, angle, cam.transform.eulerAngles.z);
    }
}

```
**添加CameraController**

将CameraController挂在对应Marker上并在Marker中添加对应事件。

<img src="document/images/6-6.png?style=centerme" height = 580/>

#### 运行 { ignore = true }

添加一个Game窗口并设置为Display2。

<img src="document/images/6-8.png?style=centerme" />

运行项目后按下F1启动鼠标模拟Marker操作（方向键左右调整MarkerID，鼠标左键放下，右键拿起，滚轮旋转），如果可以通过模拟Marker操控展示屏相机移动旋转则说明运行成功。

<img src="document/images/6-9.png?style=centerme" />
<img src="document/images/6-10.png?style=centerme" />

---

#### 双工程项目开发流程

需要分别创建**展示屏工程**和**交互屏工程**，双工程开发需要用到SDK中通信部分的内容，但它的优点是可以支持您在不同设备运行两个项目并实现交互。

##### 展示屏开发流程

##### 新建工程 { ignore = true }

创建Unity项目并根据[环境要求](#环境要求)修改配置。

#### 导入SDK { ignore = true }

按照[在Unity项目中导入SDK](#在unity项目中导入sdk)所述进行操作。

##### 添加SDK组件 { ignore = true }

**目的**：开启服务供交互屏通信使用。

将**NDITableAssets/Prefabs/**[SkyfallServer](#skyfallserver)拖拽到场景中，如图：

<img src="document/images/1-4.png?style=centerme" />

##### 在漫游相机上添加NDITableGO { ignore = true }

**目的**：方便获取场景内的物体（详情请查看：[NDITableGO](#nditablego)）

<img src="document/images/6-1.png?style=centerme" />

##### 添加校正方案 { ignore = true }

**目的**：使交互屏生成校正方案。

创建3D锚点并找三个位置（[校正方案](#校正方案)）

<img src="document/images/6-4.png?style=centerme" height = 425 />

##### 创建FreeCall脚本并添加到场景 { ignore = true }

**目的**：创建接口供交互屏通信调用

**创建FreeCall**

```csharp
using SkyfallServices;
using UnityEngine;
using xFrame.NDITable.Unity;
using xFrame.Skyfall.FreeCallManager;

public class FreeCall : MonoBehaviour {
    void Awake() {
        this.FreeCallRegister();
    }

    /// <summary>
    /// 设置漫游相机位置和旋转
    /// </summary>
    /// <param name="position">位置坐标</param>
    /// <param name="angle">旋转角度</param>
    [FreeCall("SetCameraPositionAndDirection")]
    public void SetCameraPositionAndDirection(Vec3 position, float angle) {
        var cam = NDITableGameObjectTable.Instance.GetComponent("MainCamera").GetComponent<Camera>();

        cam.transform.position = new Vector3(position.X, cam.transform.position.y, position.Z);

        cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x, angle, cam.transform.eulerAngles.z);
    }
}
```
**将FreeCall添加到场景中**

<img src="document/images/6-2.png?style=centerme" />

---

##### 交互屏开发流程

##### 新建工程 { ignore = true }

创建Unity项目并根据[环境要求](#环境要求)修改配置。

#### 导入SDK { ignore = true }

按照[在Unity项目中导入SDK](#在unity项目中导入sdk)所述进行操作。

##### 添加SDK组件 { ignore = true }

在**NDITableAssets/Prefabs**中将[NDITableClient](#nditableclient)、[SkyfallClient](#skyfallclient)拖拽到场景中，并将Unity创建UI时自动生成的EventSystem替换为[NDITableEventSystem](#nditableeventsystem)，如图：

<img src="document/images/2-1.png?style=centerme" />

##### 添加校正方案 { ignore = true }

**目的**：生成校正方案。

创建2D锚点并找到与展示屏对应的三个位置（[校正方案](#校正方案)）

<img src="document/images/6-5.png?style=centerme" />

##### 添加漫游Marker { ignore = true }

**目的**：添加Marker用于项目漫游功能实现。

将SDK中的FirstPersonMarker拖入场景并添加漫游脚本（[Markers](#markers)、[调用FreeCall](#调用freecall-仅双工程项目交互屏使用)）

**新建CameraController**

**目的**：自定义Marker事件实现漫游功能。

```csharp
using Google.Protobuf.WellKnownTypes;
using SkyfallServices;
using UnityEngine;
using xFrame.NDITable.Unity;
using xFrame.NDITable.Unity.EventSystems;

public class CameraController : MarkerController {
    //获取Client
    public SkyfallClientComponent Client;

    private bool createScheme = false;
```
```csharp

    public override void MarkerAppear(MarkerEventData data) {
        if (!Client.IsConnected())
            return;

        if (createScheme) {
            //坐标转换
            var pos = CoordMappingSchemeManager.Instance.Mapping(
                new Vector3((float)(data.Data.X / Screen.width), (1.0f - (float)(data.Data.Y / Screen.height))));

            //调用Freecall方法
            FreeCallRequest request = new FreeCallRequest();

            request.Params.Add(Any.Pack(new StringValue { Value = "SetCameraPositionAndDirection" }));
            request.Params.Add(Any.Pack(new Vec3 { X = pos.x, Y = pos.y, Z = pos.z }));
            request.Params.Add(Any.Pack(new FloatValue {
                Value = (float)data.Data.Angle + GetComponent<Marker>().AngleOffset
            }));

            Client.RequestOneway("CommonService@freeCall", request);
        }
    }

    public override void MarkerUpdate(MarkerEventData data) {
        if (!Client.IsConnected())
            return;

        if (!createScheme) {
            createScheme = true;

            //注册校正方案
            StringValue request = new StringValue {Value = "City"};

            Client.Request("CommonService@mapping", request, response => {
                var positions = (MappingPositions) response;

                string bottomAnchorName = Screen.width > 1300 ? "TouchCity" : "City";
                var anchor = AlignAnchorGroupTable.Instance.GetComponent(bottomAnchorName);

                var anchor1 = GetAnchor(anchor.Index1.GetComponent<RectTransform>().anchoredPosition);
                var anchor2 = GetAnchor(anchor.Index2.GetComponent<RectTransform>().anchoredPosition);
                var anchor3 = GetAnchor(anchor.Index3.GetComponent<RectTransform>().anchoredPosition);

                var scheme = new CoordMappingScheme(new Vector4[] {
                    anchor1,
                    anchor2,
                    anchor3
                }, new Vector4[] {
                    new Vector3(positions.Index1.X, positions.Index1.Y, positions.Index1.Z),
                    new Vector3(positions.Index2.X, positions.Index2.Y, positions.Index2.Z),
                    new Vector3(positions.Index3.X, positions.Index3.Y, positions.Index3.Z),
                });

                CoordMappingSchemeManager.Instance.Register("City", scheme);
            });
        }

        if (createScheme) {
            //坐标转换
            var pos = CoordMappingSchemeManager.Instance.Mapping(
                new Vector3((float)(data.Data.X / Screen.width), (1.0f - (float)(data.Data.Y / Screen.height))));

            //调用Freecall方法
            FreeCallRequest freeCallRequest = new FreeCallRequest();

            freeCallRequest.Params.Add(Any.Pack(new StringValue { Value = "SetCameraPositionAndDirection" }));
            freeCallRequest.Params.Add(Any.Pack(new Vec3 { X = pos.x, Y = pos.y, Z = pos.z }));
            freeCallRequest.Params.Add(Any.Pack(new FloatValue { Value = (float)data.Data.Angle + GetComponent<Marker>().AngleOffset }));

            Client.RequestOneway("CommonService@freeCall", freeCallRequest);
        }
    }

    public override void MarkerDisappear(MarkerEventData data) { }

    private static Vector3 GetAnchor(Vector3 source) {
        return new Vector3(Mathf.Abs(source.x / Screen.width), Mathf.Abs(source.y / Screen.height), 1.0f);
    }
}

```
**添加CameraController**

将CameraController挂在对应Marker上并在Marker中添加对应事件。

<img src="document/images/3-4.png?style=centerme" />

##### 运行 { ignore = true }

同时运行两个工程后，在交互屏工程中按下F1启动鼠标模拟Marker操作（方向键左右调整MarkerID，鼠标左键放下，右键拿起，滚轮旋转），如果可以通过模拟Marker操控展示屏相机移动旋转则说明运行成功。

<img src="document/images/6-11.png?style=centerme" />
<img src="document/images/6-12.png?style=centerme" />

#### 打包

在确定配置正确的情况下，将工程打出PC包。

<img src="document/images/6-3.png?style=centerme" />

>**注意**：打包格式必须是**x86_64**的格式

---

## SDK资源介绍

### 基础

#### Markers

我们提供了一些常用的Marker样式供您使用，它们在`NDITableAssets/Prefabs/Markers`，您可以直接拖拽到场景中进行使用，当然您也可以自定义Marker的样式，只需要修改动画和相关图片资源。

首先我们要了解Marker的三个响应事件：放下（MarkerAppear）、拿起（MarkerDisappear）、更新（MarkerUpdate）。当我们把Marker放在交互屏上时，会先触发`MarkerAppear`事件然后每帧触发`MarkerUpdate`事件,直到我们把Marker从交互屏上拿起并触发`MarkerDisappear`事件。

这里我们以FirstPersonMarker为例：

将FirstPersonMarker拖拽到场景中，如图：

<img src="document/images/3-1.png?style=centerme" />

因为Marker是预设模型所以需要把Canvas的RenderMode调整为`Screen Space - Camera`，如图：

<img src="document/images/3-2.png?style=centerme" />

#### 挂在Marker上的脚本介绍 {ignore = true}

**Animation Events**

封装了一些对Unity Animation的操作。

**FirstPersonMarkerController**

用来控制Marker动画的一些显隐和循环操作。

**Marker**：

用来对Marker预设进行控制的主脚本。

>Enable：是否激活Marker
>BindId：绑定的MarkerId
>MarkerFollow：Marker模型预设体是否跟随实体Marker（自定义值可获取用于实际需求）
>AngleOffset：Marker角度偏移量（自定义值可获取用于实际需求）
>DisplayCamera：Marker模型预设体使用的相机（一般情况默认为主相机MainCamera即Canvas对应相机）
>Radius：Marker模型预设体相对于实体物理Marker的大小（值越小Marker显示越大）
>MarkerAppear：实体Marker被放下时触发的事件(可自行添加删除)
>MarkerUpdate：实体Marker放下后每帧触发的事件（可自行添加删除）
>MarkerDisappear：实体Marker被拿起时触发的事件(可自行添加删除)

#### 添加进行Marker操作时的事件 {ignore = true}

新建一个脚本并把继承的`MonoBehaviour`替换为`MarkerController`并实现方法：

```csharp
using xFrame.NDITable.Unity;
using xFrame.NDITable.Unity.EventSystems;

public class MarkerEventController : MarkerController
{
    public override void MarkerAppear(MarkerEventData data) { }

    public override void MarkerDisappear(MarkerEventData data) { }

    public override void MarkerUpdate(MarkerEventData data) { }
}
```

MarkerEventData:

> data.Data.Angle => Marker当前角度
> data.Data.Id => Marker当前Id
> data.Data.State => Marker当前状态(Appear,Disappear,Update)
> data.Data.X => Marker当前位置X
> data.Data.Y => Marker当前位置Y

将刚编写的脚本挂在对应的Marker上，并在`Marker.cs`里添加对应事件，如图：

<img src="document/images/3-4.png?style=centerme" />

#### 添加Marker与UI交互的事件 { ignore = true }

场景中的UI也可以和Marker进行交互响应三个事件：Marker进入UI（MarkerEnter）、Marker持续在UI中（MarkerHover）、Marker离开UI（MarkerExit）。

用户可以对UI添加`NDITableEventTrigger`来对这三个事件进行监听

---

#### NDITableClient

用于接收Marker和触屏相关消息事件，在创建单工程项目或双工程的交互屏时需要将该预设拖入场景。

>Server Address：接收消息的设备IP地址（默认为本机即127.0.0.1）
>Port：接收端口（默认为2071）

---

#### NDITableEventSystem

替换Unity原有的EventSystem用于接收Marker和触屏相关信息，在创建单工程项目或双工程的交互屏时需要用该预设替换Unity原有的EventSystem。

---

#### 校正方案

当我们需要通过Marker控制展示屏相机进行漫游时，需要让3D场景中的位置和2D平面图的各个点产生对应关系，因此我们需要添加校正方案。

##### 添加锚点 {ignore = true }

点击`Tools/xFrame/NDITable`分别在3D场景和2D平面图中创建3D和2D锚点，如图：

<img src="document/images/5-1.png?style=centerme" />

>**注意**：2D锚点要放在对应的Canvas中,并把AnchorGroup的位置归零、大小归一。
><img src="document/images/5-4.png?style=centerme" />


##### 设置锚点位置 {ignore = true }

拖拽锚点中的Index使2D和3D锚点中相同Index的位置对应，如图：

<img src="document/images/5-2.png?style=centerme" />
<img src="document/images/5-3.png?style=centerme" />

#### 生成校正方案 {ignore = true }

获取两个锚点组(AlignAnchorGroup)生成解决方案：
```csharp
using UnityEngine;
using xFrame.NDITable.Unity;

public class AnchorRegister : MonoBehaviour
{
    void Start()
    {
        //交互屏锚点
        var anchorCityBottom = AlignAnchorGroupTable.Instance.GetComponent("TouchCity");
        //展示屏锚点
        var anchorCity = AlignAnchorGroupTable.Instance.GetComponent("City");

        var scheme = new CoordMappingScheme(new Vector4[] {
            GetAnchor(anchorCityBottom.Index1.GetComponent<RectTransform>().anchoredPosition),
            GetAnchor(anchorCityBottom.Index2.GetComponent<RectTransform>().anchoredPosition),
            GetAnchor(anchorCityBottom.Index3.GetComponent<RectTransform>().anchoredPosition)
        }, new Vector4[] {
            anchorCity.Index1.transform.position,
            anchorCity.Index2.transform.position,
            anchorCity.Index3.transform.position,
        });

        CoordMappingSchemeManager.Instance.Register("City", scheme);
    }
```
```csharp

    private static Vector3 GetAnchor(Vector3 source) {
        return new Vector3(Mathf.Abs(source.x / Screen.width), Mathf.Abs(source.y / Screen.height), 1.0f);
    }
}

```

**注意**：两工程开发时注册校正方案放在交互屏，展示屏锚点需要通过Client进行通信：

```csharp
public SkyfallClientComponent Client;

//展示屏锚点
StringValue request = new StringValue {Value = "City"};

Client.Request("CommonService@mapping", request, response => {
    var positions = (MappingPositions) response;

    //交互屏锚点
    var anchor = AlignAnchorGroupTable.Instance.GetComponent("TouchCity");

    var anchor1 = GetAnchor(anchor.Index1.GetComponent<RectTransform>().anchoredPosition);
    var anchor2 = GetAnchor(anchor.Index2.GetComponent<RectTransform>().anchoredPosition);
    var anchor3 = GetAnchor(anchor.Index3.GetComponent<RectTransform>().anchoredPosition);

    var scheme = new CoordMappingScheme(new Vector4[] {
                    anchor1,
                    anchor2,
                    anchor3
        }, new Vector4[] {
                new Vector3(positions.Index1.X, positions.Index1.Y, positions.Index1.Z),
                new Vector3(positions.Index2.X, positions.Index2.Y, positions.Index2.Z),
                new Vector3(positions.Index3.X, positions.Index3.Y, positions.Index3.Z),
            });

    CoordMappingSchemeManager.Instance.Register("City", scheme);
});
```

**注意**：注册校正方案通常放在Unity的Start()声明周期中防止注册失败。

##### 切换校正方案 {ignore = true }

如果一个工程中有多个校正方案，在把所有的校正方案都进行注册后可以通过代码进行切换：
```csharp
//name为注册时输入的名称
CoordMappingSchemeManager.Instance.Switch(string name);
```

##### 坐标转换 {ignore = true }

```csharp
//将UI上的2D坐标转换为3D场景中的坐标
Vector3 targetPos = CoordMappingSchemeManager.Instance.Mapping(Vector3 source);
```

---

#### NDITableGO

当我们需要对场景中某个物体进行操作时，我们提供了可以获取到这个物体的接口。

##### 对需要操作的物体添加脚本 {ignore = true }

在需要获取的物体上添加脚本`NDITableGO`，如图：

<img src="document/images/6-1.png?style=centerme" />

NDITableGO.cs:

>Register Name ： 获取时需要输入的名称，默认为当前物体名称。

##### 获取物体 {ignore = true }

获取物体：

```csharp
    //获取通过NDITableGO注册的物体
    NDITableGameObjectTable.Instance.GetComponent(string registerName);
```

---

### 通信部分

**注意**： 通信部分组件通常只在双工程项目中被使用，用于两个工程之间相互通信。

#### SkyfallServer

只用于双工程开发时展示屏服务的开启，在创建双工程项目的展示屏时将该预设拖入场景。

>IP Address：生成服务的IP地址（默认为本机：127.0.0.1）
>Port：生成服务的端口（默认为40000）

---

#### SkyfallClient

只用于双工程开发时交互屏连接展示屏的服务，在创建双工程项目的交互屏时将该预设拖入场景。

>Server Address：需要连接的服务端Ip地址
>Port：需要连接的服务端端口
>Expire Second：过期时间
>Timeout Second：连接超时时间

---

#### FreeCall

在双工程项目中，如果您需要添加需要交互屏交互才能调用的事件，您需要创建此脚本并挂在场景中。

新建一个脚本并在Awake中调用`this.FreeCallRegister()`注意需要引用namespace，将我们需要在交互屏调用的事件写在这个脚本里并加上特性（Attribute）：`[FreeCall("")]`，引号内为交互屏调用时需要输入的名称。

```csharp
 using xFrame.Skyfall.FreeCallManager;
 void Awake() {
        this.FreeCallRegister();
    }

    [FreeCall("ParameterlessFunction")]
    public void ParameterlessFunction() { }

    [FreeCall("PramaterFunction")]
    public void PramaterFunction(int foo, string bar) { }

    [FreeCall("HasReturnValueFunction")]
    public bool HasReturnValueFunction(int foo, string bar){
        return true;
    }
```

---

#### 调用FreeCall

进行这一步之前要在展示屏创建好我们需要调用的接口（详情请查看[FreeCall](#freecall仅双工程项目展示屏使用))，然后在需要的地方进行调用：

```csharp
//获取Client
public SkyfallClientComponent Client;

//创建FreeCallRequest
FreeCallRequest request = new FreeCallRequest();

request.Params.Add(Any.Pack(new StringValue { Value = "FunctionName" }));
request.Params.Add(Any.Pack(new Int32Value{ Value = 0 }));

//与展示屏通信(Request)
Client.RequestOneway("CommonService@freeCall", request);
```

##### 获取Client { ignore = true }

首先我们需要获取到刚刚挂在场景中的`SkyfallClient`上的脚本`SkyfallClientComponent`，您可以通过任何方式获取它：

```csharp
public SkyfallClientComponent Client;
```

##### 创建`FreeCallRequest` { ignore = true }

然后我们需要创建一个`FreeCallRequest`的对象并将展示屏定义的方法名和参数传入对象并打包。这里我们还是以展示屏定义的设置摄像机位置和旋转的方法为例：

```csharp
//创建FreeCallRequest对象
FreeCallRequest request = new FreeCallRequest();

//添加展示屏定义的方法名称
request.Params.Add(Any.Pack(new StringValue { Value = "FunctionName" }));
//添加展示屏定义的方法所需要的参数
request.Params.Add(Any.Pack(new Int32Value{ Value = 0 }));
```

将参数进行打包的类型：

>String => StringValue
>int => Int32Value
>float => FloatValue
>bool => BoolValue
>Vector2 => Vec2(需要将x,y分别传入)
>Vector3 => Vec3(需要将x,y,z分别传入)

####和展示屏通信 { ignore = true }

Client中提供了两种通信展示屏的方法分别用于有返回值的方法和没有返回值的方法。

**无返回值**

只需要将方法调用不需要拿到返回值时调用`RequestOneway`：

```csharp
//Client 为之前获取的 SkyfallClientComponent
//request 为之前定义的 FreeCallRequest

Client.RequestOneway("CommonService@freeCall", request);
```

**有返回值**

调用方法并需要获取返回值时调用`Request`

```csharp
//Client 为之前获取的 SkyfallClientComponent
//request 为之前定义的 FreeCallRequest

 Client.Request("CommonService@freeCall", request,
                response => {
                    //获取返回值并转换为返回类型（此处例子为bool）
                    var info = (FreeCallResponse)response;
                    var value = info.Results[0].Unpack<BoolValue>().Value;

                    //具体实现
                    ...
                });
```

---
