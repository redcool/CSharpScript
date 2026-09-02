# CSharpScript

[English](README.md) | [简体中文](README.zh-CN.md)

## 简介

CSharpScript 是一个基于 SlowSharp（Roslyn）的 Unity 运行时 C# 脚本系统：在编辑器和运行时（玩家端）中动态编译并执行 C# 代码，实现热更新 / 运行时 C# 脚本功能。底层依赖 SlowSharp——一个基于 Roslyn（Microsoft.CodeAnalysis）的 C# 脚本引擎，本包在 Plugins/ 目录下打包了 Slowsharp.dll、Microsoft.CodeAnalysis*.dll 以及若干 BCL 支撑库。

使用方式非常简单：把一段 C# 代码（字符串 / TextAsset / .csx 文件）交给 CScriptComponent 执行，脚本中所有继承自 UnityEngine.MonoBehaviour 的类会被实例化并绑定到 GameObject 上，Awake、Start、Update、OnGUI 等 Unity 生命周期方法会自动转发给脚本化的 MonoBehaviour，从而实现真正的运行时 C# 脚本（热更新）。

（文档：本包没有独立的外部文档链接；参考与依赖见下方“参考仓库 / 依赖”小节，底层引擎仓库为 https://github.com/redcool/SlowSharp ）

## 功能特性（Features）

- **CScriptComponent**（Core/CScriptComponent.cs）—— 在运行时执行 C# 代码，并把脚本化的 MonoBehaviour 绑定到 GameObject 上的 MonoBehaviour 组件。
  - `Run(string codeStr)` / `Run(CScript runner)` —— 创建一个 SlowSharp 的 CScript 运行器，加载脚本，然后通过 runner.Override(type.FullName, gameObject) 把所有派生自 UnityEngine.MonoBehaviour 的类型实例化并挂到该 GameObject 上（monoInstList）。
  - 生命周期转发 —— Awake、Start、Update、LateUpdate、OnGUI、OnEnable、OnDisable、OnDestroy 都会被转发给每个脚本化的 MonoBehaviour（方法清单与 Res/UnityEventsTemplate.txt 一致）。
  - InvokeMonoMethod(methodName) —— 在所有脚本化的 MonoBehaviour 上调用任意方法（结果缓存在 methodMonoInstDict 中）。
  - 静态入口 —— CScriptComponent.Run(GameObject, string)、CScriptComponent.Run(GameObject, CScript)、GetAddComponent<T>(GameObject)。
- **CScriptRunTest**（Core/CScriptRunTest.cs）—— [ExecuteAlways] 测试组件：在多行文本框 codeStr 中粘贴代码，勾选 isTest 即可编译并运行一次（Run() -> CScriptComponent.Run(gameObject, codeStr).RunMain()）。
- **CScriptUtils**（Core/CScriptUtils.cs）—— 静态辅助方法 RunScript(CScript runner, string codeStr)：加载脚本并运行其 Main（未传入运行器时自动创建一个）。
- **编辑器集成**（CScriptComponentEditor）—— 检视面板中的 “Gen Invoke Codes” 按钮：根据 TextAsset 代码模板按行生成包装方法，每行一个：void <name>() { InvokeMonoMethod(nameof(<name>)); }。
- **Plugins/** —— 打包了 SlowSharp + Microsoft.CodeAnalysis（Roslyn）运行时及其 BCL 依赖库。

## 目录结构（Folder structure）

- Core/ —— 核心脚本：CScriptComponent.cs、CScriptRunTest.cs、CScriptUtils.cs
- Plugins/ —— 插件 DLL：Slowsharp.dll、Microsoft.CodeAnalysis*.dll 及支撑 DLL（System.Memory、System.Collections.Immutable、System.Reflection.Metadata、System.Buffers、System.Numerics.Vectors、System.Threading.Tasks.Extensions、System.Runtime.CompilerServices.Unsafe）
- Res/ —— 示例资源：AComp.csx（示例脚本类）、UnityEventsTemplate.txt（被转发的 Unity 事件名）
- Test/ —— TestScript.unity（测试场景）
- link.xml —— IL2CPP 链接器配置（保留 UnityEngine.IMGUIModule）
- LICENSE —— Apache License 2.0 许可

## 使用说明（Usage）

1. 把 C# 代码写成字符串 / TextAsset / .csx 文件，例如一个继承自 MonoBehaviour 的类（参见 Res/AComp.csx）：

    public class AComp : MonoBehaviour
    {
        public void OnGUI() { GUILayout.Button("AComp onGUI .............", null); }
        public void Update() { Debug.Log("AComp update .."); }
        void Main() { Debug.Log("AComp Main test done"); }
    }

2. 执行代码：
   - 直接在 GameObject 上运行（会自动添加 CScriptComponent 组件）：
        var runner = CScriptComponent.Run(gameObject, codeStr); runner.RunMain();
   - 使用测试检视面板：添加 CScriptRunTest 组件，把代码粘贴进 codeStr，勾选 isTest。
   - 静态辅助方法：CScriptUtils.RunScript(null, codeStr);

3. 脚本化的 MonoBehaviour 会被实例化并绑定到该 GameObject 上；Unity 生命周期方法（Awake、Start、Update 等）会自动转发。需要调用脚本中的任意方法时，使用 InvokeMonoMethod(methodName)。

## 参考仓库 / 依赖（Reference Gits）

- SlowSharp（底层 C# 脚本引擎）：https://github.com/redcool/SlowSharp
- 本包仓库：https://github.com/redcool/CSharpScript.git

## 备注 / 更新记录（Notes / Changelog）

- 基于 SlowSharp，它通过 Microsoft.CodeAnalysis（Roslyn）在运行时编译 C# 代码。
- 请保留 link.xml（UnityEngine.IMGUIModule），以便 IL2CPP 构建下脚本代码可以使用 IMGUI。
- 遵循 Apache License 2.0 许可（见 LICENSE）。
