using UnityEditor;
using UnityEngine;
using unvs.editor.pythonsext;
using System.IO;

public static class PythonTest {
    [MenuItem("UNVS/Python/Run Test Script")]
    public static void RunTest() {
        string scriptPath = Path.Combine(Application.dataPath, "..", "Packages/com.unvs.core/Editor/pythons/scripts/test_script.py");
        scriptPath = Path.GetFullPath(scriptPath);

        if (!File.Exists(scriptPath)) {
            Debug.LogError($"Test script not found at: {scriptPath}");
            return;
        }

        var testParams = new {
            message = "Hello from Unity!",
            value = 42,
            isTest = true
        };

        Debug.Log("Starting Python Test...");
        PythonExt.RunPythonScript(scriptPath, testParams);
    }
}
