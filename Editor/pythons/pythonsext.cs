using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace unvs.editor.pythonsext {
    public static class PythonExt {
        /// <summary>
        /// Run python script using system process.
        /// </summary>
        /// <param name="scriptPath">The path to the python script.</param>
        /// <param name="Params">
        /// The parameters to pass to the python script this is json string or object.
        /// Python Code can access it via sys.argv[1].
        /// </param>
        public static void RunPythonScript(string scriptPath, object Params) {
            string json = string.Empty;
            if (Params is string jsonString) {
                json = jsonString;
            } else {
                json = JsonUtility.ToJson(Params);
            }

            // Prepare process start info
            ProcessStartInfo startInfo = new ProcessStartInfo {
                FileName = "python", // Assuming python is in PATH
                Arguments = $"\"{scriptPath}\" \"{json.Replace("\"", "\\\"")}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            UnityEngine.Debug.Log($"[Python] Running script: {scriptPath}");

            try {
                using (Process process = Process.Start(startInfo)) {
                    // Create task-like console output
                    process.OutputDataReceived += (sender, e) => {
                        if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.Log($"[Python] {e.Data}");
                    };
                    process.ErrorDataReceived += (sender, e) => {
                        if (!string.IsNullOrEmpty(e.Data)) UnityEngine.Debug.LogError($"[Python Error] {e.Data}");
                    };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                }
            } catch (System.Exception ex) {
                UnityEngine.Debug.LogError($"[Python] Failed to start process: {ex.Message}. Make sure 'python' is in your system PATH.");
            }
        }
    }
}