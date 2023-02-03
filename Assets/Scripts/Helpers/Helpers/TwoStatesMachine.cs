using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class TwoStatesMachine
{
    public bool? CurrentState { get; private set; }
    public bool? CurrentIntermediateState { get; private set; }
    public event System.Action<bool> OnMovedToState;

    private Func<bool, CancellationToken, UniTask> SetStateAsync;
    private Action StopSettingState;
    private Action<bool> OnBeforeSetState;
    private Action<bool> OnAfterSetState;
    private Action<bool> SetStateImmediate;
    private CancellationTokenSource moveToStateAsyncCancellationTokenSource = new CancellationTokenSource();
    public TwoStatesMachine()
    {

    }
    public TwoStatesMachine(Func<bool, CancellationToken, UniTask> setStateAsync, Action stopSettingState, Action<bool> setStateImmediate,
        Action<bool> onBeforeSetState = null, Action<bool> onAfterSetState = null)
    {
        Init(setStateAsync, stopSettingState, setStateImmediate, onBeforeSetState, onAfterSetState);
    }
    public void Init(Func<bool, CancellationToken, UniTask> setStateAsync, Action stopSettingState, Action<bool> setStateImmediate,
        Action<bool> onBeforeSetState = null, Action<bool> onAfterSetState = null)
    {
        SetStateAsync = setStateAsync;
        StopSettingState = stopSettingState;
        SetStateImmediate = setStateImmediate;
        OnBeforeSetState = onBeforeSetState;
        OnAfterSetState = onAfterSetState;
    }

    public void MoveToState(bool state)
    {
        if (SetStateImmediate == null)
        {
            Debug.LogError($"{nameof(TwoStatesMachine)} is not inited");
            return;
        }
        if(CurrentState == state)
        {
            return;
        }
        if (CurrentIntermediateState != null)
        {
            moveToStateAsyncCancellationTokenSource.Cancel();
            moveToStateAsyncCancellationTokenSource = new CancellationTokenSource();
        }
        OnBeforeSetState?.Invoke(state);
        SetStateImmediate(state);
        CurrentState = state;
        OnAfterSetState(state);
        OnMovedToState?.Invoke(state);
    }
    public async UniTask MoveToStateAsync(bool state)
    {
        if (SetStateAsync == null)
        {
            throw new Exception($"{nameof(TwoStatesMachine)} is not inited");
        }
        if(CurrentState == state)
        {
            return;
        }
        if (CurrentIntermediateState.HasValue)
        {
            if (state == CurrentIntermediateState.Value)
            {
                return;
            }
            else
            {
                moveToStateAsyncCancellationTokenSource.Cancel();
                moveToStateAsyncCancellationTokenSource = new CancellationTokenSource();
                StopSettingState();
            }
        }

        OnBeforeSetState?.Invoke(state);
        CurrentIntermediateState = state;
        CurrentState = null;
        var cancelllationToken = moveToStateAsyncCancellationTokenSource.Token;
        await SetStateAsync(state, cancelllationToken);
        if (cancelllationToken.IsCancellationRequested)
        {
            CurrentIntermediateState = null;
            return;
        }
        CurrentIntermediateState = null;
        CurrentState = state;
        OnAfterSetState?.Invoke(state);
        OnMovedToState?.Invoke(state);
    }
}
