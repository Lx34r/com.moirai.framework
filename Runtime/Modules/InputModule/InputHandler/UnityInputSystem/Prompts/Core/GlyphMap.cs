using System;
using System.Collections;
using System.Linq;
using Moirai.Atropos.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Moirai.Atropos.Input.Prompts
{
    /// <summary>
    /// 图标与 TMP Sheet 绑定
    /// </summary>
    [Serializable]
    public class PromptGlyph
    {
        [TableColumnWidth(57, Resizable = false)]
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        [SerializeField] private Sprite m_Icon;
        /// <summary>对应的图标。假设 TMP Sprite 资源和 Sprite 已同步并且有相同的名称</summary>
        public Sprite Icon => m_Icon;

        [VerticalGroup("Binding"), LabelText("Sprite Sheet Name")]
        [ReadOnly]
        [SerializeField] private string m_TextMeshSpriteSheetName;
        /// <summary>包含此字形的 text mesh sprite sheet 的名称。</summary>
        public string TextMeshSpriteSheetName { get => m_TextMeshSpriteSheetName; internal set => m_TextMeshSpriteSheetName = value; }
    }

    /// <summary>
    /// 动作的完整路径和对应图标
    /// </summary>
    [Serializable]
    public class ActionGlyph : PromptGlyph
    {
        [Tooltip("动作绑定的完整路径，例如 \"<Gamepad>/leftStick\"")]
        [VerticalGroup("Binding"), LabelText("Binding Path")]
        [SerializeField] private string m_ActionBindingPath;
        /// <summary>动作绑定的完整路径，例如 <![CDATA["<Gamepad>/leftStick"]]></summary>
        /// <remarks>详情可见 - https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/ActionBindings.html</remarks>
        public string ActionBindingPath => m_ActionBindingPath;
    }

    /// <summary>
    /// 自定义 sprite 条目，用于根据设备使用不同的图标（例如，用于控制器图标）
    /// </summary>
    [Serializable]
    public class DeviceGlyph
    {
        [Tooltip("设备类型")]
        [ValueDropdown(nameof(DeviceNames))]
        [SerializeField] private string m_DeviceName;
        /// <summary>设备类型</summary>
        public string DeviceName => m_DeviceName;

        // ReSharper disable once InconsistentNaming
        private static IEnumerable DeviceNames = new ValueDropdownList<string>()
        {
            { "Accelerometer", "Accelerometer" },
            { "Ambient Temperature Sensor", "AmbientTemperatureSensor" },
            { "Android Accelerometer", "AndroidAccelerometer" },
            { "Android Ambient Temperature", "AndroidAmbientTemperature" },
            { "Android Game Rotation Vector", "AndroidGameRotationVector" },
            { "Android Gamepad", "AndroidGamepad" },
            { "Android Gamepad With Dpad Axes", "AndroidGamepadWithDpadAxes" },
            { "Android Gamepad With Dpad Buttons", "AndroidGamepadWithDpadButtons" },
            { "Android Gravity Sensor", "AndroidGravitySensor" },
            { "Android Gyroscope", "AndroidGyroscope" },
            { "Android Hinge Angle", "AndroidHingeAngle" },
            { "Android Joystick", "AndroidJoystick" },
            { "Android Light Sensor", "AndroidLightSensor" },
            { "Android Linear Acceleration Sensor", "AndroidLinearAccelerationSensor" },
            { "Android Magnetic Field Sensor", "AndroidMagneticFieldSensor" },
            { "Android Pressure Sensor", "AndroidPressureSensor" },
            { "Android Proximity", "AndroidProximity" },
            { "Android Relative Humidity", "AndroidRelativeHumidity" },
            { "Android Rotation Vector", "AndroidRotationVector" },
            { "Android Step Counter", "AndroidStepCounter" },
            { "Attitude Sensor", "AttitudeSensor" },
            { "Daydream Controller", "DaydreamController" },
            { "Daydream HMD", "DaydreamHMD" },
            { "Dual Sense Gamepad HID", "DualSenseGamepadHID" },
            { "Dual Sense Gampadi OS", "DualSenseGampadiOS" },
            { "Dual Shock 3 Gamepad HID", "DualShock3GamepadHID" },
            { "Dual Shock 4 Gamepad Android", "DualShock4GamepadAndroid" },
            { "Dual Shock 4 Gamepad HID", "DualShock4GamepadHID" },
            { "Dual Shock 4 Gampadi OS", "DualShock4GampadiOS" },
            { "Dual Shock Gamepad", "DualShockGamepad" },
            { "Fast Keyboard", "FastKeyboard" },
            { "Fast Mouse", "FastMouse" },
            { "Fast Touchscreen", "FastTouchscreen" },
            { "Gamepad", "Gamepad" },
            { "Gear VR Tracked Controller", "GearVRTrackedController" },
            { "Gravity Sensor", "GravitySensor" },
            { "Gyroscope", "Gyroscope" },
            { "Handed Vive Tracker", "HandedViveTracker" },
            { "HID", "HID" },
            { "Hinge Angle", "HingeAngle" },
            { "Hololens Hand", "HololensHand" },
            { "Humidity Sensor", "HumiditySensor" },
            { "Input Device", "InputDevice" },
            { "IOS Game Controller", "iOSGameController" },
            { "IOS Step Counter", "iOSStepCounter" },
            { "Joystick", "Joystick" },
            { "Keyboard", "Keyboard" },
            { "Light Sensor", "LightSensor" },
            { "Linear Acceleration Sensor", "LinearAccelerationSensor" },
            { "Magnetic Field Sensor", "MagneticFieldSensor" },
            { "Mouse", "Mouse" },
            { "Nimbus Gamepad Hid", "NimbusGamepadHid" },
            { "Oculus HMD", "OculusHMD" },
            { "Oculus HMD Extended", "OculusHMDExtended" },
            { "Oculus Remote", "OculusRemote" },
            { "Oculus Touch Controller", "OculusTouchController" },
            { "Oculus Tracking Reference", "OculusTrackingReference" },
            { "Open VR Controller WMR", "OpenVRControllerWMR" },
            { "Open VR Oculus Touch Controller", "OpenVROculusTouchController" },
            { "Open VRHMD", "OpenVRHMD" },
            { "Pen", "Pen" },
            { "Pointer", "Pointer" },
            { "Pressure Sensor", "PressureSensor" },
            { "Proximity Sensor", "ProximitySensor" },
            { "Sensor", "Sensor" },
            { "Step Counter", "StepCounter" },
            { "Switch Pro Controller HID", "SwitchProControllerHID" },
            { "Touchscreen", "Touchscreen" },
            { "Tracked Device", "TrackedDevice" },
            { "Vive Lighthouse", "ViveLighthouse" },
            { "Vive Tracker", "ViveTracker" },
            { "Vive Wand", "ViveWand" },
            { "Web GL Gamepad", "WebGLGamepad" },
            { "Web GL Joystick", "WebGLJoystick" },
            { "WMR Spatial Controller", "WMRSpatialController" },
            { "WMRHMD", "WMRHMD" },
            { "X Input Controller", "XInputController" },
            { "X Input Controller Windows", "XInputControllerWindows" },
            { "Xbox One Gamepad Android", "XboxOneGamepadAndroid" },
            { "Xbox One Gampadi OS", "XboxOneGampadiOS" },
            { "XR Controller", "XRController" },
            { "XR Controller With Rumble", "XRControllerWithRumble" },
            { "XRHMD", "XRHMD" },
        };

        [Tooltip("设备图标")]
        [SerializeField] private Sprite m_DeviceSprite;
        /// <summary>设备图标</summary>
        public Sprite DeviceSprite => m_DeviceSprite;
    }

    /// <summary>
    /// 单个输入设备（如 PlayStation 4 Controller）的数据
    /// </summary>
    [CreateAssetMenu(menuName = "Moirai Framework/Input/Glyph Map", order = 1)]
    public class GlyphMap : ScriptableObject
    {
        [Tooltip("此资产支持的设备类型（可以是多个，例如 mouse/keyboard）")]
        [SerializeField] private DeviceGlyph[] m_DeviceGlyphs;
        /// <summary>此资产支持的设备类型（可以是多个，例如 mouse/keyboard）</summary>
        public DeviceGlyph[] DeviceGlyphs => m_DeviceGlyphs;

        [Tooltip("设备描述")]
        [TextAreaResizable]
        [SerializeField] private string m_DeviceDescription;
        /// <summary>设备描述</summary>
        public string DeviceDescription => m_DeviceDescription;

        [Tooltip("所有动作绑定及其相应提示图标的列表")]
        [TableList(ShowPaging = true)]
        [SerializeField] private ActionGlyph[] m_ActionGlyphs;
        /// <summary>所有动作绑定及其相应提示图标的列表</summary>
        public ActionGlyph[] ActionGlyphs => m_ActionGlyphs;

        [NonSerialized] private string[] _deviceNames;

        /// <summary>
        /// 可用于标识此设备的设备名称
        /// </summary>
        public string[] DeviceNames
        {
            get
            {
                _deviceNames ??= m_DeviceGlyphs
                    .Where(entry => !string.IsNullOrWhiteSpace(entry.DeviceName)) // 过滤空值
                    .Select(entry => entry.DeviceName.Trim()) // 去除前后空格
                    .Distinct() // 去重
                    .ToArray();

                // Log.Info($"{name}: {string.Join(", ", _deviceNames)}");

                return _deviceNames;
            }
        }
    }
}