# 性能优化

分析和优化代码的性能。

## 参数
- $TARGET: 要优化的文件或目录路径
- $FOCUS: 优化重点（可选：memory, cpu, gpu, network）

## 优化维度

### 1. 内存优化
- 减少 GC 分配
- 使用对象池
- 避免装箱拆箱
- 优化数据结构

### 2. CPU 优化
- 优化算法复杂度
- 减少循环次数
- 使用缓存
- 并行处理

### 3. GPU 优化
- 减少 Draw Call
- 优化 Shader
- 使用 LOD
- 批处理

### 4. 网络优化
- 减少网络请求
- 压缩数据
- 使用缓存
- 异步处理

## Unity 性能优化

### 1. Update 优化
```csharp
// 问题代码
void Update()
{
    // 每帧执行的逻辑
}

// 优化方案1：使用定时器
private float _timer;
private float _interval = 0.1f; // 100ms 执行一次

void Update()
{
    _timer += Time.deltaTime;
    if (_timer >= _interval)
    {
        _timer = 0;
        // 执行逻辑
    }
}

// 优化方案2：使用事件驱动
private void OnEnable()
{
    EventSystem.Subscribe<SomeEvent>(OnSomeEvent);
}

private void OnDisable()
{
    EventSystem.Unsubscribe<SomeEvent>(OnSomeEvent);
}
```

### 2. 字符串优化
```csharp
// 问题代码
void Update()
{
    string text = "Score: " + score.ToString();
}

// 优化方案：使用 StringBuilder
private StringBuilder _sb = new StringBuilder();

void Update()
{
    _sb.Clear();
    _sb.Append("Score: ");
    _sb.Append(score);
    string text = _sb.ToString();
}
```

### 3. 协程优化
```csharp
// 问题代码
IEnumerator SomeCoroutine()
{
    while (true)
    {
        yield return null; // 每帧执行
    }
}

// 优化方案：使用 UniTask
async UniTaskVoid SomeTask()
{
    while (true)
    {
        await UniTask.Yield();
        // 或者使用 UniTask.Delay
        await UniTask.Delay(100); // 100ms 执行一次
    }
}
```

### 4. 对象池优化
```csharp
// 问题代码
void SpawnObject()
{
    Instantiate(prefab, position, rotation);
}

void DestroyObject(GameObject obj)
{
    Destroy(obj);
}

// 优化方案：使用对象池
private ObjectPool<GameObject> _pool;

void SpawnObject()
{
    var obj = _pool.Get();
    obj.transform.position = position;
    obj.transform.rotation = rotation;
}

void DestroyObject(GameObject obj)
{
    _pool.Return(obj);
}
```

## 性能分析工具

### 1. Unity Profiler
- CPU Profiler
- Memory Profiler
- GPU Profiler

### 2. 代码分析
- 静态代码分析
- 运行时分析
- 内存分析

### 3. 第三方工具
- JetBrains dotMemory
- Redgate ANTS
- Visual Studio Diagnostic Tools

## 优化流程

### 1. 性能分析
- 识别瓶颈
- 收集数据
- 确定目标

### 2. 优化实施
- 选择优化策略
- 实施优化
- 验证效果

### 3. 测试验证
- 功能测试
- 性能测试
- 回归测试

## 输出格式

```markdown
## 性能优化报告

### 文件信息
- 文件路径：{file_path}
- 优化前性能：{before_performance}
- 优化后性能：{after_performance}

### 性能分析

#### 瓶颈识别
- [瓶颈1]
- [瓶颈2]

#### 优化机会
- [机会1]
- [机会2]

### 优化方案

#### 1. [优化类型]
- **问题**：[性能问题描述]
- **优化**：[优化方案]
- **收益**：[性能提升]

### 优化前后对比

#### 优化前
```csharp
// 原代码
```

#### 优化后
```csharp
// 优化后代码
```

### 性能提升
- CPU 使用率：{cpu_improvement}
- 内存使用：{memory_improvement}
- 帧率提升：{fps_improvement}

### 建议
- [进一步优化建议]

### 总结
- 性能提升：[评估]
- 代码复杂度：[评估]
- 维护性：[评估]
```

请提供要优化的代码，我会进行全面的性能优化。
