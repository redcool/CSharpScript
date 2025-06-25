namespace CSharpScript
{
    using Slowsharp;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    [ExecuteAlways]
    public class CScriptRunTest : MonoBehaviour
    {
        public bool isTest;

        [Multiline(20)]
        public string codeStr;

        public void Run()
        {
            if (string.IsNullOrEmpty(codeStr))
                return;

            CScriptComponent.Run(gameObject, codeStr).RunMain();
        }

        private void Update()
        {
            if (isTest)
            {
                isTest = false;

                Run();
            }
        }
    }
}