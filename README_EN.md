# Moirai Framework Installer

[![README](https://img.shields.io/badge/README-中文-red)](https://github.com/TeamMoirai/com.moirai.framework/blob/installer/README.md)

A lightweight, zero-dependency Unity Editor tool that provides a guided installation workflow for the [Moirai Framework](https://github.com/TeamMoirai/com.moirai.framework).

## Overview

The installer automates the entire framework setup process through a single editor window:

1. **Registry Bootstrap** — Automatically configures the OpenUPM scoped registry in your project's `manifest.json`
2. **Core Installation** — Installs the `com.moirai.framework` package via Unity Package Manager
3. **Template Import** — Copies a project template (Normal or Hybrid) into your `Assets/` folder
4. **Environment Configuration** — Sets scripting defines, player settings, and build target options

## Requirements

- **Unity** 2022.3 or newer
- **Internet access** (for OpenUPM package resolution)

## Installation

Add the installer to your project by adding the following entry to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.moirai.framework.installer": "https://github.com/TeamMoirai/com.moirai.framework.git#installer"
  }
}
```

## Usage

Open the installer window via the Unity menu:

```
Tools > Settings > Install Framework
```

### Step 1: Environment Check

The window displays a dashboard with green/red status indicators for:

| Check | Description |
|-------|-------------|
| OpenUPM Registry | Scoped registry configured in `manifest.json` |
| Unity Version | Unity 2022.3+ detected |
| Core Package | `com.moirai.framework` installed |
| URP Package | Universal Render Pipeline installed |
| HybridCLR | HybridCLR package installed (Hybrid template only) |
| Installer State | Current project state (NotInstalled / Custom / Normal / Hybrid) |

### Step 2: Install Core

Click **Install Core** to install `com.moirai.framework` via UPM. The installer monitors the async operation and updates the UI on completion.

### Step 3: Choose a Template

Two templates are available:

#### Normal Template

Standalone framework template for standard projects.

- Scripting defines: `ENABLE_LOG`
- Player settings: `allowUnsafeCode = true`

#### Hybrid Template

Hot-update template for projects using [HybridCLR](https://github.com/focus-creative-games/hybridclr).

- Scripting defines: `ENABLE_LOG`, `ENABLE_HYBRIDCLR`
- Player settings: `allowUnsafeCode = true`, IL2CPP backend, .NET Framework 4.6, incremental GC disabled
- **Requires:** HybridCLR package installed

### Step 4: Install Template

Click **Install Template** to copy the selected template into your project. Existing files are preserved — the installer will skip files that already exist and log warnings.

## Install State

The installer persists its state to:

```
ProjectSettings/MoiraiFrameworkInstaller.json
```

This file records the installer state, template type, Unity version, and timestamp. On subsequent opens, the installer validates the persisted state against actual filesystem markers to ensure consistency.

## Project Structure After Installation

After installation, your project will have the following structure (varies by template):

```
Assets/
├── Bundles/                    # Asset bundle output
├── YooAsset/                   # YooAsset configuration
├── Scripts/
│   └── Runtime/
│       ├── HotfixEntry.cs      # Hot-update entry point
│       └── GameLogic.Runtime.asmdef
└── ...

Packages/
├── com.moirai.framework/       # Core framework (installed via UPM)
└── ...

ProjectSettings/
└── MoiraiFrameworkInstaller.json  # Installer state
```

## Core Framework Dependencies

The `com.moirai.framework` package pulls in the following dependencies automatically:

| Package | Version |
|---------|---------|
| `com.cysharp.unitask` | 2.5.11 |
| `com.tuyoogame.yooasset` | 2.3.19 |
| `com.unity.scriptablebuildpipeline` | 1.21.5 |
| `com.unity.nuget.newtonsoft-json` | 3.2.1 |
| `com.unity.collections` | 2.2.1 |
| `com.unity.burst` | 1.8.9 |
| `com.unity.mathematics` | 1.3.1 |

## Menu Reference

| Menu Item | Description |
|-----------|-------------|
| `Tools > Settings > Install Framework` | Opens the installer window |

## License

[MIT](LICENSE)

## Links

- [Moirai Framework Repository](https://github.com/TeamMoirai/com.moirai.framework)
- [OpenUPM](https://openupm.com/)
- [HybridCLR](https://github.com/focus-creative-games/hybridclr)
