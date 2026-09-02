# CSharpScript

[English](README.md) | [简体中文](README.zh-CN.md)


Runtime C# scripting for Unity, based on SlowSharp (Roslyn): compile and run C# code at runtime in editor and player (hot reload / runtime C# scripting).

## Features

- **CScriptComponent** (Core/CScriptComponent.cs) — MonoBehaviour that runs C# code at runtime and binds scripted MonoBehaviours to a GameObject.
  - `Run(string codeStr)` / `Run(CScript runner)` — creates a SlowSharp CScript runner, loads the script, then instantiates every type derived from UnityEngine.MonoBehaviour onto the GameObject via runner.Override(type.FullName, gameObject) (monoInstList).
  - Lifecycle forwarding — Awake, Start, Update, LateUpdate, OnGUI, OnEnable, OnDisable, OnDestroy are forwarded to every scripted MonoBehaviour (list matches Res/UnityEventsTemplate.txt).
  - InvokeMonoMethod(methodName) — invokes any method on all scripted MonoBehaviours (results cached in methodMonoInstDict).
  - Static entry points — CScriptComponent.Run(GameObject, string), CScriptComponent.Run(GameObject, CScript), GetAddComponent<T>(GameObject).
- **CScriptRunTest** (Core/CScriptRunTest.cs) — [ExecuteAlways] test component: paste code in a multiline codeStr, tick isTest to compile & run once (Run() -> CScriptComponent.Run(gameObject, codeStr).RunMain()).
- **CScriptUtils** (Core/CScriptUtils.cs) — static helper RunScript(CScript runner, string codeStr): loads the script and runs its Main (creates a runner if none is passed).
- **Editor integration** (CScriptComponentEditor) — inspector button "Gen Invoke Codes" generates wrapper methods from a TextAsset template, one per line: void <name>() { InvokeMonoMethod(nameof(<name>)); }.
- **Plugins/** — bundles SlowSharp + Microsoft.CodeAnalysis (Roslyn) runtimes and their BCL dependencies.

## Folder structure

- Core/ — CScriptComponent.cs, CScriptRunTest.cs, CScriptUtils.cs
- Plugins/ — Slowsharp.dll, Microsoft.CodeAnalysis*.dll, and support DLLs (System.Memory, System.Collections.Immutable, System.Reflection.Metadata, System.Buffers, System.Numerics.Vectors, System.Threading.Tasks.Extensions, System.Runtime.CompilerServices.Unsafe)
- Res/ — AComp.csx (example script class), UnityEventsTemplate.txt (forwarded Unity event names)
- Test/ — TestScript.unity (test scene)
- link.xml — IL2CPP linker config (keeps UnityEngine.IMGUIModule)
- LICENSE — Apache License 2.0

## Usage

1. Write C# code as a string / TextAsset / .csx file, e.g. a class deriving from MonoBehaviour (see Res/AComp.csx):

    public class AComp : MonoBehaviour
    {
        public void OnGUI() { GUILayout.Button("AComp onGUI .............", null); }
        public void Update() { Debug.Log("AComp update .."); }
        void Main() { Debug.Log("AComp Main test done"); }
    }

2. Execute it:
   - Directly on a GameObject (auto-adds a CScriptComponent):
        var runner = CScriptComponent.Run(gameObject, codeStr); runner.RunMain();
   - With the test inspector: add CScriptRunTest, paste code into codeStr, enable isTest.
   - Static helper: CScriptUtils.RunScript(null, codeStr);

3. Scripted MonoBehaviours are instantiated and bound to the GameObject; Unity lifecycle methods (Awake, Start, Update, ...) are forwarded automatically. Use InvokeMonoMethod(methodName) to call arbitrary script methods.

## Reference Gits / Dependencies

- SlowSharp (underlying C# scripting engine): https://github.com/redcool/SlowSharp
- Package git: https://github.com/redcool/CSharpScript.git

## Notes

- Based on SlowSharp, which compiles C# via Microsoft.CodeAnalysis (Roslyn) at runtime.
- Keep link.xml (UnityEngine.IMGUIModule) for IL2CPP builds so scripted code can use IMGUI.
- Licensed under Apache License 2.0 (see LICENSE).