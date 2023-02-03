using System.Collections;
using UnityEngine;

[System.Serializable]
public class Timer
{
    public event System.Action OnCycle;
    public event System.Action OnReset;
    public event System.Action OnStart;
    public event System.Action OnStop;

    [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    public float Time { get; private set; }
    public System.TimeSpan TimeSpan => System.TimeSpan.FromSeconds(Time);

    [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    /// <summary>
    /// If true - timer will use Time.unscaledDeltaTime
    /// </summary>
    public bool UseUnscaledGlobalTime { get; set; }

    [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    public bool IsRunning { get; private set; }

    [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    public bool IsResetting { get; private set; }

    [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.ReadOnly]
    public float LocalTimeScale { get; private set; }

    public MonoBehaviour MonoBehaviour { get; set; }

    private Coroutine timerCoroutine;
    private Coroutine smoothResetCoroutine;
    private System.Action OnCycleAction;

    private float cycleTime = -1;
    private float cycleTimeElapsed;

    public Timer(bool useUnscaledGlobalTime = false)
    {
        UseUnscaledGlobalTime = useUnscaledGlobalTime;
        StopAndReset();
        SetLocalTimeScale(1);
    }

    public Timer(MonoBehaviour monoBehaviour, bool useUnscaledGlobalTime = false)
    {
        this.MonoBehaviour = monoBehaviour;
        UseUnscaledGlobalTime = useUnscaledGlobalTime;
        StopAndReset();
        SetLocalTimeScale(1);
    }

    public void SetLocalTimeScale(float timeScale)
    {
        LocalTimeScale = timeScale;
    }

    public void SetCycle(float cycleTime, System.Action OnCycleAction = null)
    {
        cycleTime = Mathf.Abs(cycleTime);
        this.cycleTime = cycleTime;
        this.OnCycleAction = OnCycleAction;
    }


    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        StopTimerCoroutine();
        StopSmoothResetCoroutine();
        IsRunning = true;
        timerCoroutine = MonoBehaviour.StartCoroutine(TimerCoroutine());
        OnStart?.Invoke();
    }

    /// <summary>
    /// Stops timer and cycle
    /// </summary>
    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }
        StopTimerCoroutine();
        StopSmoothResetCoroutine();
        IsRunning = false;
        OnStop?.Invoke();
    }

    /// <summary>
    /// Resets time and cycle
    /// </summary>
    public void Reset()
    {
        ResetTime();
        ResetCycle();
        OnReset?.Invoke();
    }
    public void ResetLerp(float speed = 1)
    {
        StopSmoothResetCoroutine();
        smoothResetCoroutine = MonoBehaviour.StartCoroutine(LerpResetCoroutine(resetSpeedMult: speed));
    }

    public void ResetSmoothLerp(float speed = 1)
    {
        StopSmoothResetCoroutine();
        smoothResetCoroutine = MonoBehaviour.StartCoroutine(LerpResetCoroutine(resetSpeedMult: speed, slerp: true));
    }

    public void ResetTime()
    {
        this.Time = 0;
    }
    public void StopCycle()
    {
        this.cycleTime = -1;
    }

    public void ResetCycle()
    {
        this.cycleTimeElapsed = 0;
    }

    public void StopAndReset()
    {
        Stop();
        Reset();
    }

    private void StopTimerCoroutine()
    {
        if (timerCoroutine != null)
        {
            MonoBehaviour.StopCoroutine(timerCoroutine);
        }

    }

    private void StopSmoothResetCoroutine()
    {
        if (smoothResetCoroutine != null)
        {
            MonoBehaviour.StopCoroutine(smoothResetCoroutine);
            IsResetting = false;
        }
    }

    private IEnumerator LerpResetCoroutine(float resetSpeedMult, bool slerp = false)
    {
        float deltaTime;
        float timeElapsed = 0;
        float initialTime = Time;
        if (initialTime <= 0)
        {
            Reset();
            yield return null;
        }
        else
        {
            while (Time > 0)
            {
                IsResetting = true;
                deltaTime = GetDeltaTime();
                timeElapsed += deltaTime;
                float t = Mathf.Clamp01(timeElapsed / initialTime) * resetSpeedMult;
                Time = slerp ? Mathf.SmoothStep(initialTime, 0, t) : Mathf.Lerp(initialTime, 0, t);
                yield return null;
            }
            Time = 0;
            Reset();
            IsResetting = false;
        }

    }


    private float GetDeltaTime()
    {
        return UseUnscaledGlobalTime ? UnityEngine.Time.unscaledDeltaTime * LocalTimeScale : UnityEngine.Time.deltaTime * LocalTimeScale;
    }

    private IEnumerator TimerCoroutine()
    {
        float deltaTime;
        while (true)
        {
            deltaTime = GetDeltaTime();
            float timeAdd = deltaTime;
            this.Time += timeAdd;
            if (cycleTime > 0)
            {
                cycleTimeElapsed += timeAdd;
                if (cycleTimeElapsed >= cycleTime)
                {
                    cycleTimeElapsed = 0;
                    OnCycleAction?.Invoke();
                    OnCycle?.Invoke();
                }
            }
            else if (cycleTime == 0)
            {
                OnCycle?.Invoke();
            }
            yield return null;
        }
    }

}
