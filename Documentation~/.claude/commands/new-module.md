# 创建新模块

在 Moirai Framework 中创建一个新的游戏模块。

## 参数
- $MODULE_NAME: 模块名称（如：QuestModule、ShopModule）

## 任务

1. **分析需求**：理解要创建的模块功能

2. **创建目录结构**：
   ```
   Assets/MoiraiFramework/Runtime/Modules/{MODULE_NAME}/
   ├── {MODULE_NAME}.cs          # 模块主类
   ├── {MODULE_NAME}Data.cs      # 模块数据（如需要）
   └── Components/               # 模块组件（如需要）
   ```

3. **生成模块代码**：
   - 继承 `Module` 基类
   - 实现 `OnInit()`、`OnShow()`、`OnHide()`、`OnUpdate()` 等生命周期方法
   - 使用 `[Module]` 特性标记
   - 遵循框架的命名规范

4. **模块模板**：
   ```csharp
   using Moirai.Runtime.Core.Attributes;

   namespace Moirai.Runtime.Modules.{MODULE_NAME}
   {
       [Module]
       public class {MODULE_NAME} : Module
       {
           protected override void OnInit()
           {
               base.OnInit();
               // 初始化逻辑
           }

           protected override void OnShow()
           {
               base.OnShow();
               // 显示时逻辑
           }

           protected override void OnHide()
           {
               base.OnHide();
               // 隐藏时逻辑
           }

           protected override void OnUpdate()
           {
               base.OnUpdate();
               // 每帧更新逻辑
           }

           protected override void OnDestroy()
           {
               base.OnDestroy();
               // 销毁清理逻辑
           }
       }
   }
   ```

5. **注册模块**：在 `GameModule.cs` 或相关配置中注册新模块

6. **创建 Editor 支持**（如需要）：
   ```
   Assets/MoiraiFramework/Editor/Modules/{MODULE_NAME}/
   ```

## 注意事项
- 遵循框架的高内聚低耦合原则
- 使用 UniTask 进行异步操作
- 使用框架的事件系统进行模块间通信
- 确保内存管理正确（对象池、资源释放）

请告诉我您要创建什么模块，我会帮您生成完整的代码。
