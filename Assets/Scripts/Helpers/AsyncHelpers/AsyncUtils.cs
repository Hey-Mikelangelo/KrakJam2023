using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
public static class AsyncUtils
{
    
    public static async UniTask SetToTargetValueAsync(System.Action<float> SetValue, float initialValue, float targetValue, float duration, 
        CancellationToken cancellationToken = default)
    {
        if (duration < 0)
        {
            Debug.LogError("duration cannot be less than 0");
            SetValue(targetValue);
            return;
        }
        float timeCounter = 0;
        SetValue(initialValue);
        while (timeCounter < duration && cancellationToken.IsCancellationRequested == false)
        {
            timeCounter += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timeCounter / duration);
            float value = Mathf.Lerp(initialValue, targetValue, t);
            SetValue(value);
            await UniTask.NextFrame(cancellationToken).SuppressCancellationThrow();
        }
        SetValue(targetValue);
    }
}
