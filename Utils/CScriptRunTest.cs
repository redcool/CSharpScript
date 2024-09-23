namespace CSharpScript
{
    using PowerUtilities;
    using Slowsharp;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class CScriptRunTest : MonoBehaviour
    {
        public string codeAbsPath;

        [EditorButton(onClickCall = "RunMain")]
        public bool isTest;

        HybInstance inst;
        public void RunMain()
        {
            var codeStr = File.ReadAllText(codeAbsPath);

            var run = CScript.CreateRunner();
            run.LoadScript(codeStr);

            inst = run.RunMain(run);
            Debug.Log(inst);
        }

    }
}