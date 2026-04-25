#if UNITY_EDITOR
using UnityEditor;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class PythonServerMenu
{
    [MenuItem("Unvs/Python/Start API Server")]
    public static void StartPythonServer()
    {
        // Đường dẫn đến file run.ps1 trong package
        // Unity sẽ hiểu đường dẫn bắt đầu bằng "Packages/com.unvs.core"
        string relativePath = "Packages/com.unvs.core/Editor/python-libs~/api-server/run.ps1";
        string fullPath = Path.GetFullPath(relativePath);

        // Kiểm tra fallback nếu package được copy vào Assets
        if (!File.Exists(fullPath))
        {
            relativePath = "Assets/com.unvs.core/Editor/python-libs~/api-server/run.ps1";
            fullPath = Path.GetFullPath(relativePath);
        }

        if (File.Exists(fullPath))
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{fullPath}\"",
                WorkingDirectory = Path.GetDirectoryName(fullPath),
                UseShellExecute = true, // Mở cửa sổ riêng để thấy log
                CreateNoWindow = false
            };

            try
            {
                Process.Start(startInfo);
                UnityEngine.Debug.Log("<color=green>Python Server:</color> Đang khởi chạy server tại " + fullPath);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("Lỗi khi khởi chạy Python Server: " + e.Message);
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Không tìm thấy file run.ps1 tại: " + fullPath + "\nBạn hãy kiểm tra lại đường dẫn folder python-libs~");
        }
    }
}
#endif
