using UnityEngine;

public struct SmoothQuaternion
{
    private Quaternion targetValue;
    public float SmoothingTime { get; set; }
    private Quaternion speed;
    public Quaternion TargetValue
    {
        get => targetValue;
        set
        {
            targetValue = value;
        }
    }
    public Quaternion Value { get; private set; }

    public SmoothQuaternion(float smoothingTime, Quaternion initialValue)
    {
        this.SmoothingTime = smoothingTime;
        targetValue = initialValue;
        Value = targetValue;
        speed = Quaternion.identity;
    }

    public void SetValueImmediate(Quaternion value)
    {
        Value = value;
        TargetValue = value;
    }
    public bool Update()
    {
        if (Value == targetValue)
        {
            return false;
        }
        Value = MathUtils.QuaternionSmoothDamp(Value, targetValue, ref speed, SmoothingTime);
        return true;
    }
}
