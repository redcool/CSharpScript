namespace CSharpScript
{
    using Slowsharp;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using System.Text;
#if UNITY_EDITOR
    using UnityEditor;
#endif

#if UNITY_EDITOR
    [CustomEditor(typeof(CScriptComponent))]
    public class CScriptComponentEditor : Editor
    {
        TextAsset codeAsset;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var inst = (CScriptComponent)target;

            EditorGUILayout.PrefixLabel("Code TextAsset","BoldLabel");
            codeAsset = (TextAsset)EditorGUILayout.ObjectField(codeAsset, typeof(TextAsset), false);
            if (codeAsset && GUILayout.Button("Gen Invoke Codes"))
                GenInvokeCode(codeAsset.text);
        }

        public void GenInvokeCode(string codeTemplate)
        {
            var lines = codeTemplate.Split('\n');
            var sb = new StringBuilder("// Generated Code\n");

            foreach (var line in lines)
            {
                var name = line.Trim();
                sb.AppendLine($"    void {name}(){{ InvokeMonoMethod(nameof({name}));}}");
            }

            Debug.Log(sb);
        }
    }
#endif


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

        static CScript runner;

        public static CScript Runner
        {
            get
            {
                if (runner == null)
                    runner = CScript.CreateRunner();
                return runner;
            }
            set { runner = value; }
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
        /// <returns>subClass of MonoBehaviour</returns>
        public int Run(CScript runner)
        {
            Runner = runner;

            if (runner == null)
                return 0;
            monoInstList.Clear();

            var types = Runner.GetTypes().Where(t => t.IsSubclassOf(typeof(MonoBehaviour)));

            foreach (var type in types)
            {
                var monoInst = Runner.Override(type.FullName, gameObject);
                monoInstList.Add(monoInst);
            }
            return monoInstList.Count;
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

        public static void Run(GameObject go, string codeStr)
        {
            go.AddComponent<CScriptComponent>().Run(codeStr);
        }

        /// <summary>
        /// Runner call script 's Main
        /// </summary>
        public static void RunMain()
        {
            Runner.RunMain();
        }

        public void InvokeMonoMethod(string methodName)
        {
            foreach (var monoInst in monoInstList)
            {
                // save method 
                if (!methodMonoInstDict.TryGetValue(methodName, out var inst))
                {
                    inst = methodMonoInstDict[methodName] = monoInst.GetMethods(methodName).Length > 0 ? monoInst : null;
                }
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