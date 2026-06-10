# 代码迁移

将代码从旧版本或旧框架迁移到 Moirai Framework。

## 参数
- $SOURCE: 源代码路径
- $TARGET: 目标路径
- $TYPE: 迁移类型（可选：framework, version, platform）

## 迁移类型

### 1. 框架迁移
- 从其他框架迁移到 Moirai
- 适配框架 API
- 重构代码结构

### 2. 版本迁移
- Unity 版本升级
- 插件版本升级
- API 变更适配

### 3. 平台迁移
- 跨平台适配
- 平台特定代码处理
- 资源格式转换

## 迁移流程

### 1. 分析阶段
- 分析源代码结构
- 识别依赖关系
- 评估迁移难度

### 2. 规划阶段
- 制定迁移计划
- 确定迁移顺序
- 识别风险点

### 3. 执行阶段
- 逐步迁移
- 保持功能不变
- 及时测试

### 4. 验证阶段
- 功能测试
- 性能测试
- 回归测试

## 框架迁移指南

### 1. 模块系统迁移
```csharp
// 旧代码
public class OldModule : MonoBehaviour
{
    void Start() { }
    void Update() { }
    void OnDestroy() { }
}

// 新代码
[Module]
public class NewModule : Module
{
    protected override void OnInit() { }
    protected override void OnUpdate() { }
    protected override void OnDestroy() { }
}
```

### 2. 事件系统迁移
```csharp
// 旧代码
public class OldEventSystem
{
    public delegate void EventHandler();
    public event EventHandler OnEvent;

    public void Raise()
    {
        OnEvent?.Invoke();
    }
}

// 新代码
public class NewEvent : IEvent { }

EventSystem.Instance.Subscribe<NewEvent>(OnEvent);
EventSystem.Instance.Publish(new NewEvent());
```

### 3. 资源管理迁移
```csharp
// 旧代码
var prefab = Resources.Load<GameObject>("path");
var instance = Instantiate(prefab);
Destroy(instance);

// 新代码
var handle = await ResourceManager.LoadAssetAsync<GameObject>("path");
var instance = handle.InstantiateAsync();
handle.Release();
```

### 4. UI 系统迁移
```csharp
// 旧代码
public class OldUI : MonoBehaviour
{
    void Show() { }
    void Hide() { }
}

// 新代码
[UIForm]
public class NewUI : UIFormLogic
{
    protected override void OnOpen() { }
    protected override void OnClose() { }
}
```

## 版本迁移指南

### 1. Unity 版本升级
- API 变更适配
- 废弃 API 替换
- 新特性采用

### 2. 插件版本升级
- API 变更适配
- 配置更新
- 功能测试

## 平台迁移指南

### 1. 平台适配
- 平台特定代码隔离
- 条件编译
- 平台接口抽象

### 2. 资源适配
- 纹理格式
- 音频格式
- 模型格式

## 输出格式

```markdown
## 迁移报告

### 迁移信息
- 源路径：{source_path}
- 目标路径：{target_path}
- 迁移类型：{migration_type}

### 迁移内容

#### 1. [迁移项]
- **源代码**：[源代码]
- **目标代码**：[目标代码]
- **变更说明**：[说明]

### 迁移统计
- 文件数量：{file_count}
- 代码行数：{line_count}
- 迁移时间：{time}

### 风险评估
- [风险1]
- [风险2]

### 测试建议
- [测试建议1]
- [测试建议2]

### 总结
- 迁移完成度：{completion}%
- 功能完整性：{functionality}%
- 建议：[后续建议]
```

请提供要迁移的代码，我会帮您完成迁移。
