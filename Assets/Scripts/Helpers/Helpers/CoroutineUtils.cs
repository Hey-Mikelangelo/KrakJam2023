using System.Collections;
using UnityEngine;
public static class CoroutineUtils
{
    public static System.Collections.IEnumerator InvokeActionWaitFrames(System.Action action, int delayFramesCount)
    {
        delayFramesCount = Mathf.Clamp(delayFramesCount, 0, int.MaxValue);
        for (int i = 0; i < delayFramesCount; i++)
        {
            yield return null;
        }
        action?.Invoke();
    }
#if UNITY_EDITOR
    public static System.Collections.IEnumerator InvokeActionWaitAllScenesLoadedEditor(System.Action action)
    {
        bool isLoadedAllScenes = false;
        while (isLoadedAllScenes == false)
        {
            isLoadedAllScenes = EditorHelpers.EditorSceneManagerExtended.IsAllScenesLoaded();
            yield return null;
        }
        if (Application.isPlaying == false)
        {
            action?.Invoke();

        }
    }
#endif
    public static IEnumerator SetValueSmooth(float from, float to, float timeToSet, System.Action<float> SetValue, System.Action OnCompleted = null, bool unscaled = false)
    {
        float t = 0;
        float time = 0;
        float val = from;
        while (t != 1)
        {
            time += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            t = Mathf.Clamp01(time / timeToSet);
            val = Mathf.Lerp(from, to, t);
            SetValue(val);
            yield return null;
        }
        SetValue(to);
        OnCompleted?.Invoke();
    }
}
