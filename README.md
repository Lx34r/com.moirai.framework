# Moirai Framework Installer

[![README](https://img.shields.io/badge/README-English-red)](https://github.com/TeamMoirai/com.moirai.framework/blob/installer/README_EN.md)

轻量、零依赖的 Unity 编辑器工具，为 [Moirai Framework](https://github.com/TeamMoirai/com.moirai.framework) 提供一键式引导安装流程。

## 概述

安装器通过单一编辑器窗口自动化整个框架搭建流程：

1. **注册表引导** — 自动在项目的 `manifest.json` 中配置 OpenUPM 作用域注册表
2. **核心安装** — 通过 Unity Package Manager 安装 `com.moirai.framework` 包
3. **模板导入** — 将项目模板（普通版或热更版）复制到 `Assets/` 目录
4. **环境配置** — 设置脚本宏定义、Player Settings 及构建目标选项

## 系统要求

- **Unity** 2022.3 或更高版本
- **网络访问**（用于 OpenUPM 包解析）

## 安装方式

在项目的 `Packages/manifest.json` 中添加以下条目：

```json
{
  "dependencies": {
    "com.moirai.framework.installer": "https://github.com/TeamMoirai/com.moirai.framework.git#installer"
  }
}
```

## 使用方法

通过 Unity 菜单打开安装器窗口：

```
Tools > Settings > Install Framework
```

### 第一步：环境检查

窗口会显示带有绿/红状态指示器的仪表盘，检查以下项目：

| 检查项 | 说明 |
|--------|------|
| OpenUPM 注册表 | `manifest.json` 中是否配置了作用域注册表 |
| Unity 版本 | 是否检测到 Unity 2022.3+ |
| 核心包 | `com.moirai.framework` 是否已安装 |
| URP 包 | Universal Render Pipeline 是否已安装 |
| HybridCLR | HybridCLR 包是否已安装（仅热更模板需要） |
| 安装器状态 | 当前项目状态（未安装 / 自定义 / 普通模板 / 热更模板） |

### 第二步：安装核心包

点击 **Install Core** 通过 UPM 安装 `com.moirai.framework`。安装器会监控异步操作并在完成后更新界面。

### 第三步：选择模板

提供两种模板可供选择：

#### 普通模板（Normal Template）

适用于标准项目的独立框架模板。

- 脚本宏定义：`ENABLE_LOG`
- Player Settings：`allowUnsafeCode = true`

#### 热更模板（Hybrid Template）

适用于使用 [HybridCLR](https://github.com/focus-creative-games/hybridclr) 的热更新项目。

- 脚本宏定义：`ENABLE_LOG`、`ENABLE_HYBRIDCLR`
- Player Settings：`allowUnsafeCode = true`、IL2CPP 后端、.NET Framework 4.6、禁用增量 GC
- **前置要求：** 需已安装 HybridCLR 包

### 第四步：安装模板

点击 **Install Template** 将所选模板复制到项目中。已有文件会被保留 — 安装器会跳过已存在的文件并输出警告日志。

## 安装状态

安装器会将状态持久化到以下文件：

```
ProjectSettings/MoiraiFrameworkInstaller.json
```

该文件记录安装器状态、模板类型、Unity 版本和时间戳。再次打开时，安装器会将持久化的状态与实际文件系统标记进行校验，以确保一致性。

## 安装后项目结构

安装完成后，项目将具有以下结构（因模板而异）：

```
Assets/
├── Bundles/                    # Asset Bundle 输出目录
├── YooAsset/                   # YooAsset 配置
├── Scripts/
│   └── Runtime/
│       ├── HotfixEntry.cs      # 热更新入口
│       └── GameLogic.Runtime.asmdef
└── ...

Packages/
├── com.moirai.framework/       # 核心框架（通过 UPM 安装）
└── ...

ProjectSettings/
└── MoiraiFrameworkInstaller.json  # 安装器状态
```

## 核心框架依赖

`com.moirai.framework` 包会自动引入以下依赖：

| 包名 | 版本 |
|------|------|
| `com.cysharp.unitask` | 2.5.11 |
| `com.tuyoogame.yooasset` | 2.3.19 |
| `com.unity.scriptablebuildpipeline` | 1.21.5 |
| `com.unity.nuget.newtonsoft-json` | 3.2.1 |
| `com.unity.collections` | 2.2.1 |
| `com.unity.burst` | 1.8.9 |
| `com.unity.mathematics` | 1.3.1 |

## 菜单参考

| 菜单项 | 说明 |
|--------|------|
| `Tools > Settings > Install Framework` | 打开安装器窗口 |

## 许可证

[MIT](LICENSE)

## 相关链接

- [Moirai Framework 仓库](https://github.com/TeamMoirai/com.moirai.framework)
- [OpenUPM](https://openupm.com/)
- [HybridCLR](https://github.com/focus-creative-games/hybridclr)
