# 修复 Bug

分析和修复代码中的 Bug。

## 参数
- $BUG_DESC: Bug 描述
- $FILE: 相关文件路径（可选）

## 修复流程

### 1. 问题分析
- 收集错误信息
- 复现问题
- 分析根本原因

### 2. 定位问题
- 检查相关代码
- 分析调用栈
- 使用调试工具

### 3. 修复方案
- 设计修复方案
- 评估影响范围
- 选择最佳方案

### 4. 实施修复
- 编写修复代码
- 添加测试用例
- 验证修复效果

### 5. 预防措施
- 添加防御性编程
- 改进错误处理
- 更新文档

## 常见 Bug 类型

### 1. 空引用异常
```csharp
// 问题代码
public void DoSomething()
{
    _target.DoAction(); // _target 可能为 null
}

// 修复代码
public void DoSomething()
{
    if (_target != null)
    {
        _target.DoAction();
    }
}
```

### 2. 资源泄漏
```csharp
// 问题代码
public void LoadResource()
{
    var resource = Resources.Load("path");
    // 忘记释放
}

// 修复代码
public async UniTask LoadResource()
{
    var resource = await Resources.LoadAsync("path");
    // 使用后释放
    Resources.UnloadAsset(resource);
}
```

### 3. 逻辑错误
```csharp
// 问题代码
public bool IsInRange(float value, float min, float max)
{
    return value >= min && value <= max; // 边界条件错误
}

// 修复代码
public bool IsInRange(float value, float min, float max)
{
    return value >= min && value < max; // 正确的边界条件
}
```

### 4. 线程安全问题
```csharp
// 问题代码
private int _counter;
public void Increment()
{
    _counter++; // 非线程安全
}

// 修复代码
private int _counter;
private readonly object _lock = new object();
public void Increment()
{
    lock (_lock)
    {
        _counter++;
    }
}
```

### 5. Unity 特定问题
- 协程未正确停止
- 对象池未正确回收
- 事件未正确注销
- 资源未正确卸载

## 输出格式

```markdown
## Bug 修复报告

### Bug 信息
- **描述**：{bug_description}
- **严重程度**：{severity}
- **影响范围**：{impact}

### 问题分析
- **根本原因**：{root_cause}
- **触发条件**：{trigger_conditions}
- **复现步骤**：{reproduction_steps}

### 修复方案
- **修复思路**：{fix_approach}
- **修改文件**：{modified_files}
- **修改内容**：{changes}

### 修复代码
```csharp
// 修复后的代码
```

### 测试验证
- [测试用例1]
- [测试用例2]

### 预防措施
- [预防措施1]
- [预防措施2]

### 总结
- 修复效果：[评估]
- 影响范围：[评估]
- 建议：[后续建议]
```

请描述您遇到的 Bug，我会帮您分析和修复。
