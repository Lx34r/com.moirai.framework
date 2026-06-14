Moirai Framework
===

[![Unity Version](https://img.shields.io/badge/Unity-2022.3%2B-blue.svg?style=flat-square)](https://unity3d.com/)
[![Releases](https://img.shields.io/github/release/TeamMoirai/com.moirai.framework.svg?style=flat-square)](https://github.com/TeamMoirai/com.moirai.framework/releases)
[![License](https://img.shields.io/github/license/TeamMoirai/com.moirai.framework?style=flat-square)](LICENSE)
[![Issues](https://img.shields.io/github/issues/TeamMoirai/com.moirai.framework?style=flat-square)](https://github.com/TeamMoirai/com.moirai.framework/issues)
[![Last Commit](https://img.shields.io/github/last-commit/TeamMoirai/com.moirai.framework?style=flat-square)](https://github.com/TeamMoirai/com.moirai.framework)
[![Top Language](https://img.shields.io/github/languages/top/TeamMoirai/com.moirai.framework?style=flat-square)](https://github.com/TeamMoirai/com.moirai.framework)
[![README](https://img.shields.io/badge/README-English-FFA500?style=flat-square)](https://github.com/TeamMoirai/com.moirai.framework/blob/main/README_EN.md)
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/TeamMoirai/com.moirai.framework)

## 📖 简介

**Moirai Framework** 是一个简单（新手友好、开箱即用）且强大的 Unity 框架全平台解决方案。

### ✨ 核心特性

- 🚀 **开箱即用** - 5 分钟即可上手整套开发流程，代码整洁，思路清晰
- 🔥 **高性能** - 基于 UniTask 的异步系统，零 GC 事件分发，严格的内存管理
- 🧩 **高内聚低耦合** - 模块化设计，可轻松移除或替换不需要的模块
- 🔄 **热更新支持** - 集成 HybridCLR，全平台热更新流程已跑通
- 📦 **资源管理** - 集成 YooAsset，支持 LRU、ARC 缓存策略，自动资源释放
- 📊 **配置表系统** - 集成 Luban，支持懒加载、异步加载、同步加载
- 🎨 **UI 框架** - 商业化 UI 开发流程，支持代码自动生成
- 🌍 **全平台支持** - Windows、Android、iOS、WebGL、微信小游戏等

---

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## 📚 目录

- [🚀 快速开始](#-%E5%BF%AB%E9%80%9F%E5%BC%80%E5%A7%8B)
  - [环境要求](#%E7%8E%AF%E5%A2%83%E8%A6%81%E6%B1%82)
  - [快速上手](#%E5%BF%AB%E9%80%9F%E4%B8%8A%E6%89%8B)
    - [下载安装](#%E4%B8%8B%E8%BD%BD%E5%AE%89%E8%A3%85)
      - [方式一：一键安装（推荐）](#%E6%96%B9%E5%BC%8F%E4%B8%80%E4%B8%80%E9%94%AE%E5%AE%89%E8%A3%85%E6%8E%A8%E8%8D%90)
      - [方式二：UPM 安装](#%E6%96%B9%E5%BC%8F%E4%BA%8Cupm-%E5%AE%89%E8%A3%85)
      - [方式三：手动安装](#%E6%96%B9%E5%BC%8F%E4%B8%89%E6%89%8B%E5%8A%A8%E5%AE%89%E8%A3%85)
    - [初始设置](#%E5%88%9D%E5%A7%8B%E8%AE%BE%E7%BD%AE)
      - [场景打包](#%E5%9C%BA%E6%99%AF%E6%89%93%E5%8C%85)
      - [配置表模块](#%E9%85%8D%E7%BD%AE%E8%A1%A8%E6%A8%A1%E5%9D%97)
    - [快捷功能](#%E5%BF%AB%E6%8D%B7%E5%8A%9F%E8%83%BD)
- [🏗️ 架构](#-%E6%9E%B6%E6%9E%84)
  - [模块系统](#%E6%A8%A1%E5%9D%97%E7%B3%BB%E7%BB%9F)
  - [启动流程](#%E5%90%AF%E5%8A%A8%E6%B5%81%E7%A8%8B)
- [📦 核心模块](#-%E6%A0%B8%E5%BF%83%E6%A8%A1%E5%9D%97)
  - [Resource — 资源管理](#resource--%E8%B5%84%E6%BA%90%E7%AE%A1%E7%90%86)
  - [UI — 界面框架](#ui--%E7%95%8C%E9%9D%A2%E6%A1%86%E6%9E%B6)
  - [Audio — 音频系统](#audio--%E9%9F%B3%E9%A2%91%E7%B3%BB%E7%BB%9F)
  - [Localization — 本地化](#localization--%E6%9C%AC%E5%9C%B0%E5%8C%96)
  - [Events — 事件系统](#events--%E4%BA%8B%E4%BB%B6%E7%B3%BB%E7%BB%9F)
  - [FSM — 有限状态机](#fsm--%E6%9C%89%E9%99%90%E7%8A%B6%E6%80%81%E6%9C%BA)
  - [Procedure — 流程管理](#procedure--%E6%B5%81%E7%A8%8B%E7%AE%A1%E7%90%86)
  - [Input — 输入系统](#input--%E8%BE%93%E5%85%A5%E7%B3%BB%E7%BB%9F)
  - [Save — 存档系统](#save--%E5%AD%98%E6%A1%A3%E7%B3%BB%E7%BB%9F)
  - [Scheduler — 调度器](#scheduler--%E8%B0%83%E5%BA%A6%E5%99%A8)
- [🧰 核心工具](#-%E6%A0%B8%E5%BF%83%E5%B7%A5%E5%85%B7)
  - [ObjectPool — 对象池](#objectpool--%E5%AF%B9%E8%B1%A1%E6%B1%A0)
  - [MemoryPool — 内存池](#memorypool--%E5%86%85%E5%AD%98%E6%B1%A0)
  - [Singleton — 单例系统](#singleton--%E5%8D%95%E4%BE%8B%E7%B3%BB%E7%BB%9F)
  - [GameConfig — 配表管理](#gameconfig--%E9%85%8D%E8%A1%A8%E7%AE%A1%E7%90%86)
  - [GameLog — 日志系统](#gamelog--%E6%97%A5%E5%BF%97%E7%B3%BB%E7%BB%9F)
  - [GameSettings — 游戏设置](#gamesettings--%E6%B8%B8%E6%88%8F%E8%AE%BE%E7%BD%AE)
  - [DataStructure — 数据结构](#datastructure--%E6%95%B0%E6%8D%AE%E7%BB%93%E6%9E%84)
- [🛠️ 编辑器工具](#-%E7%BC%96%E8%BE%91%E5%99%A8%E5%B7%A5%E5%85%B7)
- [📁 推荐项目结构](#-%E6%8E%A8%E8%8D%90%E9%A1%B9%E7%9B%AE%E7%BB%93%E6%9E%84)
- [🤝 贡献与支持](#-%E8%B4%A1%E7%8C%AE%E4%B8%8E%E6%94%AF%E6%8C%81)
  - [🌟 生态依赖](#-%E7%94%9F%E6%80%81%E4%BE%9D%E8%B5%96)
  - [👥 贡献者](#-%E8%B4%A1%E7%8C%AE%E8%80%85)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## 🚀 快速开始

### 环境要求

- **Unity 版本**: 2022.3.x（推荐）或更高

- **开发环境**: .NET 4.x

- **支持平台**: Windows、OSX、Android、iOS、WebGL

### 快速上手

#### 下载安装

##### 方式一：一键安装（推荐）

1. 通过以下任一方式安装 **Framework Installer**：

   - 在 **Window/Package Manager** 中，通过 Git URL 安装：

      ```bash
      https://github.com/TeamMoirai/com.moirai.framework.git#installer
      ```
      <img src="Documentation~\src\quick-start-1.png" alt="quick-start-scoped-registries" />
   
   - 克隆 `install` 分支至工程目录（Assets/...）：

      ```bash
      git clone --branch installer --single-branch https://github.com/TeamMoirai/com.moirai.framework.git Scripts/Installer
      ```
   
2. 回到 Unity，手动执行菜单 `Tools/Settings/Install Framework`

3. 安装完成后可安全删除该脚本

##### 方式二：UPM 安装

1. 在 **Project Settings/Unity Package Manager** 中，手动添加 **Scoped Registry**：

   ```text
   // 输入以下内容（国际版）
   Name: Open UPM
   URL: https://package.openupm.com
   Scope(s): com.cysharp
   		  com.tuyoogame
   		  com.moirai
   ```

   <img src="Documentation~\src\quick-start-2-scoped-registries.png" alt="quick-start-scoped-registries" />

2. 在 **Window/Package Manager** 中，选中 **Moirai Framework**，点击 **Install** 进行安装：

    <img src="Documentation~\src\quick-start-2-package-detail.png" alt="quick-start-package-detail" />

3. 手动复制 `工程根目录/Library/PackageCach/com.moirai.framework@xxx/Templates~/` 下 **@Requirements** 文件夹内的所有内容到 **工程根目录/Assets** 目录。

    （可选）根据需要选择同目录下合适的模板复制到工程，一般选择 **NormalTemplate** 即可。

##### 方式三：手动安装

1. 以下获取框架的方式任选其一：

   - 在 **Window/Package Manager** 中，通过 Git URL 安装：

     ```
     https://github.com/TeamMoirai/com.moirai.framework.git
     ```

   - 在发布的Release版本中，选择最新版本下载 **Source Code** 压缩包。

2. 参见 **方式二：UPM 安装.3**。

---

#### 初始设置

##### 场景打包

将 `Scenes/main.unity` 加入打包

- Unity6.0+： `File -> Build Profiles -> SceneList`
- Unity6.0-：`File -> Build Settings -> Scene In Build`

##### 配置表模块

   - 选择 `Tools/Settings/ConfigTableSettings` ，点击 `生成 Config 到指定目录`。
   - 如果移动配置表目录，则需要在  `Tools/Settings/ConfigTableSettings` 手动更新——`重定向 Config 目录`

---

#### 快捷功能

1. **编辑器模式运行**
   - 选择顶部菜单栏 `YooAsset/Editor PlayMode` 编辑器下的模拟模式
   - 点击 `Play` 开始运行
2. **打包运行**（热更新流程）
   - 运行菜单 `HybridCLR/Install...` 安装 HybridCLR
   - 运行菜单 `HybridCLR/Define Symbols/Enable HybridCLR` 开启热更新
   - 运行菜单 `HybridCLR/Generate/All` 进行必要的生成操作
   - 运行菜单 `HybridCLR/Build/BuildAssets And CopyTo AssemblyPath` 生成热更新 DLL
   - 运行菜单 `YooAsset/AssetBundle Builder` 构建 AB
   - 打开 Build Settings，点击 Build And Run

> 💡 **提示**: 遇到问题请查看 [HybridCLR 常见错误](https://hybridclr.doc.code-philosophy.com/docs/help/commonerrors)

---

## 🏗️ 架构

```
com.moirai.framework/
├── Runtime/              # 核心框架程序集 (Moirai.Atropos)
│   ├── Core/             # 基础工具与数据结构
│   │   ├── Events/       #   事件系统（池化、冒泡传播）
│   │   ├── Schedulers/   #   零分配调度器（定时器/帧计数器）
│   │   ├── Singleton/    #   单例系统（纯 C# / MonoBehaviour）
│   │   ├── MemoryPool/   #   内存池
│   │   ├── Pool/         #   对象池（通用/UniTask/GameObject）
│   │   ├── Tween/        #   缓动系统
│   │   ├── Tasks/        #   任务/序列系统
│   │   ├── GameConfig/   #   Luban 配置表管理
│   │   ├── GameLog/      #   日志系统
│   │   ├── GameSettings/ #   画面设置
│   │   ├── DataStructure/#   IOC容器、优先队列、稀疏数组等
│   │   └── Extension/    #   通用扩展方法
│   └── Modules/          #   功能模块
│       ├── ResourceModule/    # YooAsset 资源管理
│       ├── UIModule/          # UI 框架（窗口/控件/层）
│       ├── AudioModule/       # 音频系统（分类/代理/淡入淡出）
│       ├── LocalizationModule/# 本地化（文本/图片/音频/Google翻译）
│       ├── FsmModule/         # 有限状态机
│       ├── ProcedureModule/   # 流程管理
│       ├── InputModule/       # 输入系统（键鼠/手柄/移动端）
│       ├── SaveModule/        # 存档系统（JSON/二进制/加密）
│       ├── ObjectPoolModule/  # 对象池模块
│       ├── SceneModule/       # 场景管理
│       ├── TimerModule/       # 计时器
│       ├── DebuggerModule/    # 运行时调试器
│       └── UpdateDriver/      # 更新循环驱动
├── Editor/               # 编辑器工具集
├── Plugins/              # 第三方库
├── Samples~/             # 示例
├── Templates~/           # 项目初始模板
└── Tests/                # 单元测试
```

### 模块系统

框架采用**模块化架构**，所有子系统均为普通 C# 类（非 MonoBehaviour），通过 `ModuleSystem` 统一注册管理。入口为 `GameModule`（MonoBehaviour），提供所有模块的静态访问器。

```csharp
// 模块访问 — 懒加载，首次访问时自动创建
var resource = GameModule.Resource;
var ui = GameModule.UI;
var audio = GameModule.Audio;
var localization = GameModule.Localization;
```

**模块生命周期：**
- `OnInit()` — 模块初始化
- `Shutdown()` — 模块销毁
- 支持 `IUpdateModule`、`IFixedUpdateModule`、`ILateUpdateModule` 接口注册到驱动循环
- 通过 `Priority` 属性控制更新顺序

### 启动流程

`Main/Procedure/` 定义了完整的启动链：

```
ProcedureLaunch → ProcedureSplash → ProcedureInitPackage → ProcedureInitResources
→ ProcedureCreateDownloader → ProcedureDownloadFile → ProcedureDownloadOver
→ ProcedureClearCache → ProcedureLoadAssembly → ProcedurePreload → ProcedurePrepare4Main
```

每个阶段均为独立的 `ProcedureBase` 状态，可通过 `ProcedureSettings`（ScriptableObject）自定义。

---

## 📦 核心模块

### Resource — 资源管理

封装 YooAsset，提供同步/异步加载 API。

| 特性 | 说明 |
|------|------|
| 播放模式 | EditorSimulate、Offline、Host、Web |
| 引用计数 | `AssetObject` 池自动管理 |
| 加载取消 | 支持 `CancellationToken` |
| 资源加密 | FileOffset、FileStream |
| 精灵加载 | `ResourceExtComponent` 支持子精灵 |

```csharp
// 异步加载
var handle = GameModule.Resource.LoadAssetAsync<GameObject>("Assets/Prefabs/Hero.prefab");
await handle.ToUniTask();
var prefab = handle.AssetObject;

// 同步加载
var sprite = GameModule.Resource.LoadAsset<Sprite>("Assets/UI/icon.png");
```

### UI — 界面框架

商业化 UI 开发流程，栈式窗口管理。

| 层级 | 用途 |
|------|------|
| Background | 背景层 |
| UI | 主界面 |
| Popup | 弹窗层 |
| System | 系统层 |
| Top | 置顶层 |

```csharp
// 打开窗口
GameModule.UI.ShowWindow<MainWindow>();

// 关闭窗口
GameModule.UI.CloseWindow<MainWindow>();

// Widget 子控件
public class MainWindow : UIWindow
{
    protected override void OnCreate() { /* 初始化 */ }
    protected override void OnRefresh(object userData) { /* 刷新数据 */ }
    protected override void OnClose() { /* 关闭清理 */ }
}
```

**编辑器支持：** `ScriptAutoGenerator` 自动生成 UI 绑定代码。

### Audio — 音频系统

分类管理、代理播放、事件驱动。

```csharp
// 播放音效
GameModule.Audio.Play("BGM_Main", AudioGroup.BGM);

// 淡入淡出
GameModule.Audio.FadeIn("BGM_Battle", 2.0f);
GameModule.Audio.FadeOut("BGM_Main", 1.5f);
```

- **AudioCategory** — 每个 AudioGroupConfig 对应一个分类
- **AudioAgent** — 代理播放，自动管理 AudioSource 生命周期
- **AudioMixer** — 集成 Unity AudioMixer
- **事件驱动** — `AudioPlayEvent`、`AudioControlEvent`、`AudioTrackEvent`、`AudioFadeEvent`

### Localization — 本地化

支持多种内容类型自动注入。

| 类型 | Localizer |
|------|-----------|
| UGUI Text | `LocalizerText` |
| TextMeshPro | `LocalizerTMPText` |
| SpriteRenderer | `LocalizerSprite` |
| RawImage / Texture | `LocalizerRawImage` |
| AudioSource | `LocalizerAudio` |
| Timeline | `LocalizerTimeline` |

- 基于 Luban 配置表加载本地化字符串
- 支持 Google 翻译集成（`GoogleTranslator`）
- 语言检测：命令行 → 编辑器设置 → 保存设置 → 系统语言

### Events — 事件系统

移植自 Unity UIElements 的池化冒泡事件系统。

```csharp
// 注册事件
EventManager.RegisterCallback<GameStartEvent>(OnGameStart);

// 发送事件（支持冒泡/捕获传播）
EventManager.SendEvent(new GameStartEvent());

// 取消注册
EventManager.UnregisterCallback<GameStartEvent>(OnGameStart);
```

- **零 GC 分配** — 每种事件类型独立对象池
- **传播机制** — TrickleDown（捕获）→ BubbleUp（冒泡）
- **传播控制** — `StopPropagation()`、`StopImmediatePropagation()`、`PreventDefault()`
- **编辑器调试** — 可视化事件派发调试窗口

### FSM — 有限状态机

```csharp
var fsm = GameModule.Fsm.CreateFsm("GameFlow", state1, state2, state3);
fsm.Start<State1>();

// 状态切换
fsm.ChangeState<State2>();

// 状态接口
public class State1 : FsmState
{
    protected override void OnInit(IFsm fsm) { }
    protected override void OnEnter(IFsm fsm) { }
    protected override void OnUpdate(IFsm fsm, float elapseSeconds) { }
    protected override void OnLeave(IFsm fsm, bool isShutdown) { }
    protected override void OnDestroy(IFsm fsm) { }
}
```

### Procedure — 流程管理

基于 FSM 的游戏流程管理，每个阶段为独立状态。

```csharp
public class ProcedurePreload : ProcedureBase
{
    protected override void OnEnter(IFsm<IProcedureModule> fsm)
    {
        // 预加载资源、初始化游戏数据
    }

    protected override void OnUpdate(IFsm<IProcedureModule> fsm, float elapseSeconds)
    {
        // 检查加载完成，切换到下一阶段
    }
}
```

### Input — 输入系统

抽象输入层，支持多平台。

| Handler | 说明 |
|---------|------|
| `UnityInputSystemHandler` | 新版 Input System |
| `UnityInputManagerHandler` | 旧版 Input Manager |
| `UIMobileInputHandler` | 移动端 UI 触控 |

- UI 模态检测 — 弹窗打开时自动屏蔽玩家输入
- 按键提示系统 — 支持 PS4/PS5/Xbox/Switch/SteamDeck/键鼠图标

### Save — 存档系统

可插拔的存档策略。

```csharp
// 保存
GameModule.Save.Save("player_data", playerData);

// 加载
var data = GameModule.Save.Load<PlayerData>("player_data");
```

| Handler | 说明 |
|---------|------|
| `JsonSaveHandler` | JSON 格式 |
| `BinarySaveHandler` | 二进制格式 |
| `JsonEncryptedSaveHandler` | JSON 加密 |
| `BinaryEncryptedSaveHandler` | 二进制加密 |

- **原子保存** — 先写 `.tmp` 再重命名，防止数据损坏

### Scheduler — 调度器

零分配的定时器/帧调度系统。

```csharp
// 延迟执行
Scheduler.Delay(2.0f, () => Debug.Log("2秒后执行"));

// 等待帧
Scheduler.WaitFrame(3, () => Debug.Log("3帧后执行"));

// 取消调度
var handle = Scheduler.Delay(5.0f, () => { });
handle.Cancel();
```

- 支持 Update / FixedUpdate / LateUpdate 帧
- 支持循环、忽略 TimeScale
- `SchedulerUnsafeBinding` — unsafe 结构体实现零分配函数指针派发

---

## 🧰 核心工具

### ObjectPool — 对象池

```csharp
// 通用对象池
var pool = new ObjectPool<MyClass>(() => new MyClass(), 32);
var obj = pool.Get();
pool.Release(obj);

// GameObject 池
var go = GameObjectPoolManager.Get("Particle");
GameObjectPoolManager.Release(go);
```

### MemoryPool — 内存池

带集合管理的泛型内存池，减少 GC 压力。

### Singleton — 单例系统

| 类型 | 说明 |
|------|------|
| `Singleton<T>` | 纯 C# 单例，双重检查锁 |
| `SingletonMono<T>` | MonoBehaviour 单例，支持持久化/替换策略 |

`SingletonSystem` 集中管理所有单例生命周期，挂接 UpdateDriver 驱动 IUpdate/IFixedUpdate/ILateUpdate。

### GameConfig — 配表管理

集成 Luban 配置表系统。

```csharp
ConfigMgr.LoadTables();           // 同步加载
await ConfigMgr.LoadTablesAsync(); // 异步加载
var cfg = ConfigMgr.Tables.TbItem.Get(itemId); // 懒加载访问
```

### GameLog — 日志系统

```csharp
Log.Info("玩家登录: {0}", playerName);
Log.Warning("资源加载失败: {0}", path);
Log.Error("严重错误!");
```

- 条件编译：`LOG_DEBUG_ENABLE`、`LOG_ALL`、`LOG_INFO_ENABLE` 等
- 可插拔 `ILogHelper` 实现自定义日志输出

### GameSettings — 游戏设置

画面设置管理：分辨率、全屏、VSync、窗口模式等。

### DataStructure — 数据结构

| 数据结构 | 说明 |
|----------|------|
| `IOCContainer` | 控制反转容器 |
| `PriorityQueue<T>` | 优先队列 |
| `RandomList<T>` | 随机列表 |
| `SerializableDictionary<K,V>` | 可序列化字典 |
| `SparseArray<T>` | 稀疏数组 |
| `ShuffleBag<T>` | 洗牌袋（不重复随机） |
| `GameDictionary<K,V>` | 游戏字典（带遍历支持） |
| `GameLinkedList<T>` | 游戏链表 |
| `GameMultiDictionary<K,V>` | 多值字典 |

---

## 🛠️ 编辑器工具

| 工具 | 用途 |
|------|------|
| Atlas Maker | Sprite Atlas 创建 |
| Custom Attributes | ~20 个自定义属性绘制器 + Odin 扩展 |
| Define Symbols | Debug/Log/Profiler 宏定义管理 |
| Event Debugger | 可视化事件派发调试窗口 |
| Game Settings | 音频组、流程设置、更新设置编辑器 |
| HybridCLR | 热更新 DLL 构建命令 |
| Luban Tools | Luban 配置表生成 |
| Maintenance | 清理空文件夹、查找丢失脚本、分组选择、锁定 Inspector |
| Reference Finder | 资源依赖/引用树视图 |
| Release Tools | 构建流水线窗口、构建配置 |
| Scheduler Debugger | 可视化调度器/计时器调试器 |
| Tasks Editor | 任务运行器编辑器 |
| UI Module | UI 绑定代码自动生成、组件 Inspector |
| Input Module | 输入动作配置编辑器、按键图标集合编辑器 |
| YooAsset | 构建缓存清理、内置目录、自定义构建管线、Shader 变体收集 |

---

##  📁 推荐项目结构

  ```text
  Project Name/
  ├── Client/                        # Unity 客户端工程
  │   └── Assets/
  │       ├── AssetArt/              # 美术资源目录
  │   		└── Atlas/             # 自动生成图集目录
  │       ├── AssetRaw/              # 热更资源目录
  │       │   ├── Audio/             # 音频资源
  │       │   ├── Config/            # 配置和本地化资源
  │       │   ├── DLL/               # 热更程序集资源
  │       │   ├── Scene/             # 资源场景
  │       │   └── UI/                # UI 预制体
  │       ├── Editor/                # 项目编辑器脚本
  │       ├── HybridCLRData/     	   # HybridCLR 生成内容
  │       ├── Scenes/                # 启动场景
  │       ├── Scripts/
  │       │   ├── GameBase/          # 主程序程序集（启动器与流程）
  │       │   ├── GameLib/           # 第三方库程序集 [Dll]
  │       │   ├── GameLogic/         # 游戏业务逻辑程序集 [Dll]
  │       │       ├── HotfixEntry.cs     # 热更主入口
  │       │   └── GameProto/         # 游戏配置协议程序集 [Dll]
  │       └── YooAsset/              # YooAsset 配置
  └── Config/                        # 配置表工程
  ```

---

## 🤝 贡献与支持

### 🌟 生态依赖

| 项目 | 描述 |
|------|------|
| **[TEngine](https://github.com/Alex-Rachel/TEngine)** | Unity 商用级别开发框架 |
| **[YooAsset](https://github.com/tuyoogame/YooAsset)** | 商业级经历百万 DAU 游戏验证的资源管理系统 |
| **[HybridCLR](https://github.com/focus-creative-games/hybridclr)** | 特性完整、零成本、高性能、低内存的近乎完美的 Unity 全平台原生 C# 热更方案。 |
| **[Luban](https://github.com/focus-creative-games/luban)** | 最佳游戏配置解决方案 |

### 👥 贡献者
[![Contributors](https://contrib.rocks/image?repo=TeamMoirai/com.moirai.framework)](https://github.com/TeamMoirai/com.moirai.framework/graphs/contributors)
