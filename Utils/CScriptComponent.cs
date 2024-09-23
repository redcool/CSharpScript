namespace CSharpScript
{

    using PowerUtilities;
    using Slowsharp;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using System.Text;

    /// <summary>
    /// Call csharpScript's MonoBehaviour
    /// </summary>
    public class CScriptComponent : MonoBehaviour
    {
        public List<HybInstance> monoInstList = new List<HybInstance>();
        /// <summary>
        /// {methodName : hubInstance}
        /// </summary>
        Dictionary<string, HybInstance> methodMonoInstDict = new Dictionary<string, HybInstance>();

        CScript runner;

        [Header("Debug")]
        public string codeAbsPath;

        [EditorButton(onClickCall = "OnRun")]
        public bool isRun;
        [Tooltip("Generate invoke code,copy code from Console")]
        public string unityEventNames = "Awake,Start,Update,LateUpdate,OnGUI,OnEnable,OnDisable,OnDestroy";

        [EditorButton(onClickCall = "OnCreateInvoke")]
        public bool isCreateInvoke;

        public CScript Runner
        {
            get
            {
                if (runner == null)
                    runner = CScript.CreateRunner();
                return runner;
            }
        }

        void OnRun()
        {
            var codeStr = File.ReadAllText(codeAbsPath);
            Run(codeStr);
        }

        public void Run(string codeStr)
        {
            Runner.LoadScript(codeStr);
            Run(Runner);
        }

        /// <summary>
        /// Run all subclass of MonoBehaviour
        /// </summary>
        /// <param name="runner"></param>
        public void Run(CScript runner)
        {
            this.runner = runner;
            if (runner == null)
                return;
            var types = Runner.GetTypes().Where(t => t.IsSubclassOf(typeof(MonoBehaviour)));
            foreach (var type in types)
            {
                var monoInst = Runner.Override(type.FullName, gameObject);
                monoInstList.Add(monoInst);
            }
        }

        /// <summary>
        /// direct run on go
        /// </summary>
        /// <param name="go"></param>
        /// <param name="runner"></param>
        public static void Run(GameObject go, CScript runner)
        {
            go.AddComponent<CScriptComponent>().Run(runner);
        }

#if UNITY_EDITOR
        void OnCreateInvoke()
        {
            var sb = new StringBuilder("// Generated Code");
            var names = unityEventNames.SplitBy();

            foreach (var name in names)
            {
                sb.AppendLine($"    void {name}(){{ InvokeMonoMethod(nameof({name}));}}");
            }

            Debug.Log(sb);
        }
#endif

        public void InvokeMonoMethod(string methodName)
        {
            foreach (var monoInst in monoInstList)
            {
                // save method 
                var inst = DictionaryTools.Get(methodMonoInstDict, methodName, (k) => monoInst.GetMethods(k).Length > 0 ? monoInst : null);
                // invoke
                inst?.Invoke(methodName);
            }
        }

        void Awake() { InvokeMonoMethod(nameof(Awake)); }
        void Start() { InvokeMonoMethod(nameof(Start)); }
        void Update() { InvokeMonoMethod(nameof(Update)); }
        void LateUpdate() { InvokeMonoMethod(nameof(LateUpdate)); }
        void OnGUI() { InvokeMonoMethod(nameof(OnGUI)); }
        void OnEnable() { InvokeMonoMethod(nameof(OnEnable)); }
        void OnDisable() { InvokeMonoMethod(nameof(OnDisable)); }
        void OnDestroy() { InvokeMonoMethod(nameof(OnDestroy)); }

    }
}