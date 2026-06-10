# Moirai Framework - Claude Code 配置

## 项目概述

**Moirai Framework** 是一个 Unity 游戏开发框架，提供模块化、高性能的开发解决方案。

### 核心特性
- 🚀 开箱即用 - 5 分钟快速上手
- 🔥 高性能 - 基于 UniTask 的异步系统，零 GC 事件分发
- 🧩 高内聚低耦合 - 模块化设计
- 🔄 热更新支持 - 集成 HybridCLR
- 📦 资源管理 - 集成 YooAsset
- 📊 配置表系统 - 集成 Luban
- 🎨 UI 框架 - 商业化 UI 开发流程

## 项目结构

```
Game Dev Tools/
├── Assets/
│   ├── MoiraiFramework/          # 核心框架
│   │   ├── Runtime/              # 运行时代码
│   │   │   ├── Core/             # 核心系统
│   │   │   └── Modules/          # 功能模块
│   │   ├── Editor/               # 编辑器代码
│   │   └── Tests/                # 测试代码
│   ├── Plugins/                  # 第三方插件
│   └── Settings/                 # 项目设置
├── Packages/                     # Unity 包
└── ProjectSettings/              # 项目配置
```

## 核心模块

### Runtime Modules
- **AudioModule** - 音频管理
- **DebuggerModule** - 调试工具
- **FsmModule** - 有限状态机
- **InputModule** - 输入系统
- **LocalizationModule** - 本地化
- **ObjectPoolModule** - 对象池
- **ProcedureModule** - 流程管理
- **ResourceModule** - 资源管理
- **SaveModule** - 存档系统
- **SceneModule** - 场景管理
- **TimerModule** - 定时器
- **UIModule** - UI 框架
- **UpdateDriver** - 更新驱动

### Core 系统
- **Attributes** - 自定义特性
- **Events** - 事件系统
- **Extension** - 扩展方法
- **GameConfig** - 游戏配置
- **GameLog** - 日志系统
- **MemoryPool** - 内存池
- **Pool** - 通用池
- **Singleton** - 单例模式
- **Tasks** - 任务系统
- **Tween** - 缓动系统
- **Utility** - 工具类

## 编码规范

### 命名规范
- **类名**：PascalCase（如：`GameModule`、`UIForm`）
- **方法名**：PascalCase（如：`OnInit`、`OnUpdate`）
- **变量名**：camelCase（如：`_timer`、`_isActive`）
- **常量**：UPPER_SNAKE_CASE（如：`MAX_COUNT`）
- **接口**：I 前缀 + PascalCase（如：`IModule`、`IEvent`）

### 代码组织
- 使用 `#region` 折叠代码块
- 按功能分组方法
- 使用 XML 文档注释

### 异步编程
- 优先使用 UniTask 而不是协程
- 使用 `async/await` 模式
- 正确处理取消和超时

### 事件系统
- 使用框架的事件系统进行模块间通信
- 避免直接引用
- 及时取消事件订阅

### 资源管理
- 使用框架的资源管理系统
- 及时释放不需要的资源
- 使用对象池管理频繁创建的对象

## Claude Code Skills

项目提供以下 Skills（通过 `/` 命令调用）：

| 命令 | 功能 |
|------|------|
| `/new-module` | 创建新模块 |
| `/new-ui` | 创建新 UI |
| `/review` | 代码审查 |
| `/explain` | 解释代码 |
| `/refactor` | 重构代码 |
| `/fix-bug` | 修复 Bug |
| `/add-event` | 添加事件 |
| `/generate-docs` | 生成文档 |
| `/test` | 生成和运行测试 |
| `/optimize` | 性能优化 |
| `/migrate` | 代码迁移 |

## 开发流程

### 1. 新功能开发
1. 确定功能需求
2. 设计模块结构
3. 使用 `/new-module` 创建模块
4. 实现功能逻辑
5. 使用 `/review` 审查代码
6. 使用 `/test` 生成测试

### 2. Bug 修复
1. 使用 `/fix-bug` 分析问题
2. 定位根本原因
3. 实施修复
4. 使用 `/test` 验证修复

### 3. 代码优化
1. 使用 `/optimize` 分析性能
2. 识别瓶颈
3. 实施优化
4. 使用 `/review` 验证优化

## 依赖项

### 核心依赖
- **UniTask** - 异步编程
- **YooAsset** - 资源管理
- **HybridCLR** - 热更新
- **Luban** - 配置表
- **R3** - 响应式编程

### 开发工具
- **Odin Inspector** - 编辑器增强
- **DOTween** - 动画系统
- **TextMesh Pro** - 文本渲染

## 注意事项

1. **Unity 版本**：推荐 Unity 2022.3.x
2. **.NET 版本**：使用 .NET 4.x
3. **平台支持**：Windows、Android、iOS、WebGL
4. **热更新**：使用 HybridCLR 进行热更新
5. **资源管理**：使用 YooAsset 管理资源

## 常见问题

### Q: 如何添加新模块？
A: 使用 `/new-module` 命令，按照模板创建模块。

### Q: 如何进行热更新？
A: 参考 README 中的打包运行步骤。

### Q: 如何优化性能？
A: 使用 `/optimize` 命令分析和优化代码。

### Q: 如何修复 Bug？
A: 使用 `/fix-bug` 命令分析和修复问题。

## 相关资源

- [Moirai Framework GitHub](https://github.com/Lx34r/com.moirai.framework)
- [YooAsset 文档](https://www.yooasset.com/)
- [HybridCLR 文档](https://hybridclr.doc.code-philosophy.com/)
- [Luban 文档](https://focus-creative-games.github.io/luban-doc/)
- [UniTask 文档](https://github.com/Cysharp/UniTask)
