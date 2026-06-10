# 添加事件

在 Moirai Framework 中添加新的事件系统。

## 参数
- $EVENT_NAME: 事件名称（如：PlayerDeath、ScoreChanged）
- $MODULE: 所属模块（可选）

## 任务

### 1. 分析事件需求
- 事件触发条件
- 事件携带数据
- 事件订阅者

### 2. 定义事件数据
```csharp
// 事件数据类
public class {EVENT_NAME}EventArgs : EventArgs
{
    public {EVENT_NAME}EventArgs(/* 参数 */)
    {
        // 初始化数据
    }

    // 事件数据属性
}
```

### 3. 创建事件管理器
```csharp
// 事件管理器
public static class {EVENT_NAME}Event
{
    // 事件定义
    public static event EventHandler<{EVENT_NAME}EventArgs> On{EVENT_NAME};

    // 触发事件
    public static void Raise({EVENT_NAME}EventArgs args)
    {
        On{EVENT_NAME}?.Invoke(null, args);
    }

    // 订阅事件
    public static void Subscribe(EventHandler<{EVENT_NAME}EventArgs> handler)
    {
        On{EVENT_NAME} += handler;
    }

    // 取消订阅
    public static void Unsubscribe(EventHandler<{EVENT_NAME}EventArgs> handler)
    {
        On{EVENT_NAME} -= handler;
    }
}
```

### 4. 注册事件
```csharp
// 在模块中注册事件
public class SomeModule : Module
{
    protected override void OnInit()
    {
        base.OnInit();
        {EVENT_NAME}Event.Subscribe(On{EVENT_NAME});
    }

    protected override void OnDestroy()
    {
        {EVENT_NAME}Event.Unsubscribe(On{EVENT_NAME});
        base.OnDestroy();
    }

    private void On{EVENT_NAME}(object sender, {EVENT_NAME}EventArgs e)
    {
        // 处理事件
    }
}
```

### 5. 触发事件
```csharp
// 在适当的地方触发事件
public class SomeClass
{
    public void DoSomething()
    {
        // 执行操作

        // 触发事件
        {EVENT_NAME}Event.Raise(new {EVENT_NAME}EventArgs(/* 参数 */));
    }
}
```

### 6. 事件总线集成（可选）
```csharp
// 使用框架的事件总线
public class {EVENT_NAME}Event : IEvent
{
    // 事件数据
}

// 注册事件
EventSystem.Instance.Subscribe<{EVENT_NAME}Event>(On{EVENT_NAME});

// 触发事件
EventSystem.Instance.Publish(new {EVENT_NAME}Event(/* 数据 */));
```

## 事件设计原则

### 1. 单一职责
- 每个事件只做一件事
- 避免事件过于复杂

### 2. 松耦合
- 发布者不需要知道订阅者
- 订阅者不需要知道发布者

### 3. 明确命名
- 使用清晰的事件名称
- 遵循命名规范

### 4. 数据封装
- 事件数据应该封装
- 避免传递过多参数

## 输出格式

```markdown
## 事件实现报告

### 事件信息
- **事件名称**：{event_name}
- **所属模块**：{module}
- **触发条件**：{trigger_condition}

### 实现代码

#### 事件数据类
```csharp
// 代码
```

#### 事件管理器
```csharp
// 代码
```

#### 使用示例
```csharp
// 代码
```

### 注意事项
- [注意事项1]
- [注意事项2]

### 测试建议
- [测试建议1]
- [测试建议2]
```

请告诉我您要添加什么事件，我会帮您实现完整的事件系统。
