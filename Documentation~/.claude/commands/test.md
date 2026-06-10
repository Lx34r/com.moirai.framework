# 生成和运行测试

为指定的代码生成单元测试并运行。

## 参数
- $TARGET: 要测试的文件或类
- $TYPE: 测试类型（可选：unit, integration, performance）

## 测试类型

### 1. 单元测试
- 测试单个方法或函数
- 隔离依赖
- 快速执行

### 2. 集成测试
- 测试模块间交互
- 测试真实依赖
- 验证集成点

### 3. 性能测试
- 测试执行时间
- 测试内存使用
- 测试并发性能

## 测试框架

### Unity Test Framework
```csharp
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;

[TestFixture]
public class {ClassName}Tests
{
    [Test]
    public void MethodName_Scenario_ExpectedBehavior()
    {
        // Arrange
        var sut = new {ClassName}();

        // Act
        var result = sut.MethodName();

        // Assert
        Assert.AreEqual(expected, result);
    }

    [UnityTest]
    public IEnumerator AsyncMethod_ShouldComplete()
    {
        // Arrange
        var sut = new {ClassName}();

        // Act
        yield return sut.AsyncMethod();

        // Assert
        Assert.IsTrue(sut.IsComplete);
    }
}
```

### UniTask 测试
```csharp
[Test]
public async UniTask AsyncMethod_ShouldReturnCorrectValue()
{
    // Arrange
    var sut = new {ClassName}();

    // Act
    var result = await sut.AsyncMethod();

    // Assert
    Assert.AreEqual(expected, result);
}
```

## 测试生成流程

### 1. 分析代码
- 识别公共方法
- 分析依赖关系
- 确定测试点

### 2. 生成测试
- 创建测试类
- 生成测试方法
- 添加测试数据

### 3. 运行测试
- 执行测试
- 收集结果
- 生成报告

## 测试最佳实践

### 1. AAA 模式
```csharp
[Test]
public void Test()
{
    // Arrange - 准备测试数据
    var sut = new Calculator();

    // Act - 执行被测试方法
    var result = sut.Add(2, 3);

    // Assert - 验证结果
    Assert.AreEqual(5, result);
}
```

### 2. 命名规范
```
MethodName_Scenario_ExpectedBehavior
```

### 3. 测试覆盖
- 正常流程
- 边界条件
- 异常情况

### 4. 测试隔离
- 每个测试独立
- 避免测试间依赖
- 使用 Setup 和 Teardown

## 输出格式

```markdown
## 测试报告

### 测试信息
- 测试文件：{test_file}
- 测试类型：{test_type}
- 测试数量：{test_count}

### 测试结果
- ✅ 通过：{pass_count}
- ❌ 失败：{fail_count}
- ⏭️ 跳过：{skip_count}

### 测试详情

#### ✅ TestName1
- 执行时间：{time}ms
- 状态：通过

#### ❌ TestName2
- 执行时间：{time}ms
- 状态：失败
- 错误信息：{error_message}
- 堆栈跟踪：{stack_trace}

### 覆盖率
- 行覆盖率：{line_coverage}%
- 分支覆盖率：{branch_coverage}%
- 方法覆盖率：{method_coverage}%

### 建议
- [改进建议1]
- [改进建议2]
```

请提供要测试的代码，我会生成完整的测试套件。
