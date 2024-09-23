using System.Collections;
namespace CSharpScript
{

    using System.Collections.Generic;
    using UnityEngine;
    using Slowsharp;
    using System;


    public static class CScriptUtils
    {

        public static void RunScript(string codeStr)
        {
            if (string.IsNullOrEmpty(codeStr))
                return;

            var run = CScript.CreateRunner();
            run.LoadScript(codeStr);
            run.RunMain(run);

        }

    }
}