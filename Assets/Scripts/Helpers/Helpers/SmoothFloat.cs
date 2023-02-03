using UnityEngine;


public struct SmoothFloat
{
    private float targetValue;
    public float SmoothingTime { get; set; }
    private float speed;
    public float TargetValue
    {
        get => targetValue;
        set
        {
            targetValue = value;
        }
    }

    public bool IsReachedTargetValue => Value == TargetValue;
    public float Value { get; private set; }

    public SmoothFloat(float smoothingTime, float initialValue = 0)
    {
        this.SmoothingTime = smoothingTime;
        targetValue = initialValue;
        Value = targetValue;
        speed = 0;
    }

    public void SetValueImmediate(float value)
    {
        Value = value;
        TargetValue = value;
    }
    public bool Update()
    {
        if(Value == targetValue)
        {
            return false;
        }
        Value = Mathf.SmoothDamp(Value, targetValue, ref speed, SmoothingTime);
        return true;
    }
}
