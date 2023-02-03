using UnityEngine;

public struct SmoothVector3
{
    public delegate Vector3 ValueGetterFunc();
    
    private Vector3 targetValue;
    public float SmoothingTime { get; set; }
    private Vector3 speed;
    public Vector3 TargetValue
    {
        get => targetValue;
        set
        {
            targetValue = value;
        }
    }
    public Vector3 Value { get; private set; }
    private Vector3 ActualValue => actualValueGetter != null ? actualValueGetter.Invoke() : Value;
    private ValueGetterFunc actualValueGetter;
    public SmoothVector3(float smoothingTime, Vector3 initialValue, ValueGetterFunc actualValueGetter = null, Vector3? speed = null)
    {
        this.SmoothingTime = smoothingTime;
        targetValue = initialValue;
        Value = initialValue;
        this.speed = speed.HasValue ? speed.Value : Vector3.zero;
        this.actualValueGetter = actualValueGetter;
    }

    public void SetValueImmediate(Vector3 value)
    {
        Value = value;
        TargetValue = value;
    }
    public bool Update()
    {
        if(ActualValue == targetValue)
        {
            return false;
        }
        Value = Vector3.SmoothDamp(Value, targetValue, ref speed, SmoothingTime, float.MaxValue, Time.deltaTime);
        return true;
    }
}
