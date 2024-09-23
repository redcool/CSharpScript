using System.Diagnostics;
using UnityEngine;
using CSharpScript;

class Foo
{
    public class AComp : MonoBehaviour
    {
        public void OnGUI()
        {
            GUILayout.Button("onGUI .............",null);
        }

        public void Update()
        {
            Debug.Log("update ..");
        }
    }

    /// args[0] : CScript(runner)
    string Main(params object[] args)
    {
        var runner = args[0];
        var go = new GameObject("test script");
        CScriptComponent.Run(go, runner);
        //var drive = go.AddComponent<CScriptComponent>();
        //drive.Run(args[0]);

        return "done";
    }
}