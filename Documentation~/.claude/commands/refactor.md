# 重构代码

对指定的代码进行重构，提高代码质量和可维护性。

## 参数
- $TARGET: 要重构的文件或目录路径
- $STYLE: 重构风格（可选：clean, performance, maintainability）

## 重构类型

### 1. 代码结构重构
- 提取方法（Extract Method）
- 提取类（Extract Class）
- 内联方法（Inline Method）
- 移动方法（Move Method）

### 2. 数据重构
- 封装字段（Encapsulate Field）
- 替换数据值为对象（Replace Data Value with Object）
- 将引用对象改为值对象（Change Reference to Value）

### 3. 条件逻辑重构
- 分解条件表达式（Decompose Conditional）
- 合并条件表达式（Consolidate Conditional Expression）
- 以卫语句取代嵌套条件（Replace Nested Conditional with Guard Clauses）

### 4. 方法重构
- 以查询取代临时变量（Replace Temp with Query）
- 引入参数对象（Introduce Parameter Object）
- 保持对象完整（Preserve Whole Object）

### 5. 类重构
- 搬移函数（Move Function）
- 将类内联化（Inline Class）
- 隐藏委托（Hide Delegate）

### 6. Unity 特定重构
- 优化 Update 方法
- 使用对象池
- 异步操作优化
- 资源管理优化

## 重构流程

1. **分析阶段**：
   - 识别代码异味（Code Smells）
   - 分析依赖关系
   - 评估重构风险

2. **重构阶段**：
   - 小步重构
   - 保持功能不变
   - 及时测试

3. **验证阶段**：
   - 运行测试
   - 检查性能
   - 验证功能

## 输出格式

```markdown
## 重构报告

### 文件信息
- 文件路径：{file_path}
- 重构前代码行数：{before_lines}
- 重构后代码行数：{after_lines}

### 重构内容

#### 1. [重构类型]
- **问题**：[代码问题描述]
- **重构**：[重构方案]
- **收益**：[重构收益]

### 重构前后对比

#### 重构前
```csharp
// 原代码
```

#### 重构后
```csharp
// 重构后代码
```

### 改进点
- [改进1]
- [改进2]

### 测试建议
- [测试建议]

### 总结
- 代码质量提升：[评估]
- 可维护性提升：[评估]
- 性能影响：[评估]
```

请提供要重构的代码，我会进行专业的重构。
