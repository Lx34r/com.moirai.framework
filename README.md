<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## 📚目录

- [📖 简介](#-%E7%AE%80%E4%BB%8B)
  - [✨ 核心特性](#-%E6%A0%B8%E5%BF%83%E7%89%B9%E6%80%A7)
- [📚 目录](#-%E7%9B%AE%E5%BD%95)
- [🚀 快速开始](#-%E5%BF%AB%E9%80%9F%E5%BC%80%E5%A7%8B)
  - [环境要求](#%E7%8E%AF%E5%A2%83%E8%A6%81%E6%B1%82)
  - [快速上手](#%E5%BF%AB%E9%80%9F%E4%B8%8A%E6%89%8B)
    - [下载安装](#%E4%B8%8B%E8%BD%BD%E5%AE%89%E8%A3%85)
      - [方式一：一键安装（推荐）](#%E6%96%B9%E5%BC%8F%E4%B8%80%E4%B8%80%E9%94%AE%E5%AE%89%E8%A3%85%E6%8E%A8%E8%8D%90)
      - [方式二：UPM 安装](#%E6%96%B9%E5%BC%8F%E4%BA%8Cupm-%E5%AE%89%E8%A3%85)
      - [方式三：手动安装](#%E6%96%B9%E5%BC%8F%E4%B8%89%E6%89%8B%E5%8A%A8%E5%AE%89%E8%A3%85)
    - [快捷功能](#%E5%BF%AB%E6%8D%B7%E5%8A%9F%E8%83%BD)
- [🤝 贡献与支持](#-%E8%B4%A1%E7%8C%AE%E4%B8%8E%E6%94%AF%E6%8C%81)
  - [🌟 生态依赖](#-%E7%94%9F%E6%80%81%E4%BE%9D%E8%B5%96)
  - [👥 贡献者](#-%E8%B4%A1%E7%8C%AE%E8%80%85)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

<div align="center">

![Logo](Documentation~/src/logo.jpg)

[![Unity Version](https://img.shields.io/badge/Unity-2022.3%2B-blue.svg?style=flat-square)](https://unity3d.com/)
[![License](https://img.shields.io/github/license/Lx34r/com.moirai.framework?style=flat-square)](LICENSE)
[![Last Commit](https://img.shields.io/github/last-commit/Lx34r/com.moirai.framework?style=flat-square)](https://github.com/Lx34r/com.moirai.framework)
[![Issues](https://img.shields.io/github/issues/Lx34r/com.moirai.framework?style=flat-square)](https://github.com/Lx34r/com.moirai.framework/issues)
[![Top Language](https://img.shields.io/github/languages/top/Lx34r/com.moirai.framework?style=flat-square)](https://github.com/Lx34r/com.moirai.framework)
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/Lx34r/com.moirai.framework)

</div>

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

## 📚 目录

- [快速开始](#-快速开始)
- [贡献与支持](#-贡献与支持)

---

## 🚀 快速开始

### 环境要求

- **Unity 版本**: 2022.3.x（推荐）或更高
- **开发环境**: .NET 4.x
- **支持平台**: Windows、OSX、Android、iOS、WebGL

### 快速上手

#### 下载安装

##### 方式一：一键安装（推荐）

1. 克隆 `install` 分支至工程目录

   ```bash
   git clone --branch installer --single-branch https://github.com/Lx34r/com.moirai.framework.git FrameworkInstaller
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

##### 方式三：手动安装

​	在发布的Release版本中，选择最新版本下载 **Source Code** 压缩包。

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

## 🤝 贡献与支持

### 🌟 生态依赖

| 项目 | 描述 |
|------|------|
| **[TEngine](https://github.com/Alex-Rachel/TEngine)** | Unity 商用级别开发框架 |
| **[YooAsset](https://github.com/tuyoogame/YooAsset)** | 商业级经历百万 DAU 游戏验证的资源管理系统 |
| **[HybridCLR](https://github.com/focus-creative-games/hybridclr)** | 特性完整、零成本、高性能、低内存的近乎完美的 Unity 全平台原生 C# 热更方案。 |
| **[Luban](https://github.com/focus-creative-games/luban)** | 最佳游戏配置解决方案 |

### 👥 贡献者
[![Contributors](https://contrib.rocks/image?repo=Lx34r/com.moirai.framework)](https://github.com/Lx34r/com.moirai.framework/graphs/contributors)
