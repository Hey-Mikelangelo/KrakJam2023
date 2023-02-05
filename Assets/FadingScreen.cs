using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class FadingScreen :  MonoBehaviour
{
    [SerializeField, Required] private ColorFadingBehaviour fadingBehaviourScreen;
    [SerializeField, Required] private Canvas canvas;
    [SerializeField, Min(0)] private float fadeDuration;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private void Awake()
    {
        this.canvas.enabled = false;
        FadeFromBlockingViewImmediate();

    }

    private void OnDestroy()
    {
        cancellationTokenSource.Cancel();
    }
    public void FadeToBlockingViewImmediate()
    {
        fadingBehaviourScreen.SetFadeValue01(1);
    }

    public void FadeFromBlockingViewImmediate()
    {
        fadingBehaviourScreen.SetFadeValue01(0);
    }

    [Button]
    public async UniTask FadeToBlockingView(float? fadeDuration = null)
    {
        if (canvas.IsNullWithErrorLog())
        {
            return;
        }
        this.canvas.enabled = true;
        if (fadeDuration.HasValue == false)
        {
            fadeDuration = this.fadeDuration;
        }
        await AsyncUtils.SetToTargetValueAsync(fadingBehaviourScreen.SetFadeValue01,
            fadingBehaviourScreen.FadeValue, 1, fadeDuration.Value, cancellationTokenSource.Token);
    }

    [Button]
    public async UniTask FadeFromBlockingView(float? fadeDuration = null)
    {
        if (fadeDuration.HasValue == false)
        {
            fadeDuration = this.fadeDuration;
        }
        await AsyncUtils.SetToTargetValueAsync(fadingBehaviourScreen.SetFadeValue01,
            fadingBehaviourScreen.FadeValue, 0, fadeDuration.Value, cancellationTokenSource.Token);
        if (canvas.Is_Not_NullWithErrorLog())
        {
            this.canvas.enabled = false;
        }
    }

}
