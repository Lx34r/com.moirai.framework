# 创建新 UI

在 Moirai Framework 的 UIModule 中创建新的 UI 界面。

## 参数
- $UI_NAME: UI 名称（如：MainMenu、SettingsPanel、BattleHUD）

## 任务

1. **分析 UI 需求**：理解 UI 的功能和布局

2. **创建 UI 目录结构**：
   ```
   Assets/MoiraiFramework/Runtime/Modules/UIModule/
   ├── Forms/
   │   └── {UI_NAME}Form.cs      # UI 表单类
   └── Components/                # UI 组件（如需要）

   Assets/资源/UI/
   ├── Prefabs/
   │   └── {UI_NAME}.prefab      # UI 预制体
   └── Textures/                  # UI 纹理
   ```

3. **生成 UI 表单代码**：
   - 继承 `UIForm` 或 `UIFormLogic`
   - 实现 UI 生命周期方法
   - 使用 `[UIForm]` 特性标记

4. **UI 表单模板**：
   ```csharp
   using Moirai.Runtime.Modules.UIModule;

   namespace Moirai.Runtime.Modules.UIModule.Forms
   {
       [UIForm]
       public class {UI_NAME}Form : UIFormLogic
       {
           // UI 组件引用
           private Button _startButton;
           private Text _titleText;

           protected override void OnInit()
           {
               base.OnInit();
               // 绑定 UI 组件
               _startButton = GetButton("StartButton");
               _titleText = GetText("TitleText");

               // 注册点击事件
               _startButton.onClick.AddListener(OnStartClick);
           }

           protected override void OnOpen()
           {
               base.OnOpen();
               // UI 打开时逻辑
           }

           protected override void OnClose()
           {
               base.OnClose();
               // UI 关闭时逻辑
           }

           protected override void OnUpdate()
           {
               base.OnUpdate();
               // UI 更新逻辑
           }

           private void OnStartClick()
           {
               // 按钮点击处理
           }
       }
   }
   ```

5. **创建 UI 组件**（如需要）：
   - 自定义 UI 组件
   - 数据绑定组件
   - 动画组件

6. **配置 UI 资源**：
   - 创建 UI 预制体
   - 配置 UI 资源路径
   - 设置 UI 层级

## 注意事项
- 使用框架的 UI 资源管理
- 遵循 UI 的命名规范
- 考虑 UI 的性能优化（对象池、懒加载）
- 支持多分辨率适配
- 使用框架的事件系统进行 UI 间通信

请告诉我您要创建什么 UI，我会帮您生成完整的代码。
