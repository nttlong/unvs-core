using System.Collections;
using UnityEngine;
using System.Diagnostics;
namespace unvs.shares{
public static class QuitManager
{
    /// <summary>
    /// Quit game normally. If still not closed after timeout, force kill process (Windows only).
    /// </summary>
    public static void Quit(float forceKillAfterSeconds = 3.0f)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit gracefully first
        Application.Quit();

        // Force kill fallback (only makes sense on standalone desktop)
        CoroutineRunner.Run(ForceKillAfterDelay(forceKillAfterSeconds));
#endif
    }

    private static IEnumerator ForceKillAfterDelay(float seconds)
    {
        if (seconds <= 0) yield break;

        float start = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - start < seconds)
            yield return null;

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX
        try
        {
            Process.GetCurrentProcess().Kill();
        }
        catch
        {
            // ignored
        }
#endif
    }
}

/// <summary>
/// Helper to run coroutine from static class.
/// </summary>
public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static void Run(IEnumerator routine)
    {
        if (_instance == null)
        {
            var go = new GameObject("[CoroutineRunner]");
            Object.DontDestroyOnLoad(go);
            _instance = go.AddComponent<CoroutineRunner>();
        }

        _instance.StartCoroutine(routine);
    }
}
}