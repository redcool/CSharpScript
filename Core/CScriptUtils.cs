using System.Collections;
namespace CSharpScript
{

    using System.Collections.Generic;
    using UnityEngine;
    using Slowsharp;
    using System;


    public static class CScriptUtils
    {

        public static void RunScript(CScript runner, string codeStr)
        {
            if (string.IsNullOrEmpty(codeStr))
                return;

            var run = runner ?? CScript.CreateRunner();
            run.LoadScript(codeStr);
            run.RunMain(run);

        }


    }
}