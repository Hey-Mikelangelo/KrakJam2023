using System;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

[Serializable]
public class IntValueSOVariable : IVariable, IVariableValueChanged
{
    public IntValueSO intValueSO;

    public event Action<IVariable> ValueChanged;

    
    public object GetSourceValue(ISelectorInfo selector)
    {
        intValueSO.OnValueChanged -= IntValueSO_OnValueChanged;
        intValueSO.OnValueChanged += IntValueSO_OnValueChanged;
        return intValueSO.IsNullWithErrorLog() ? "Null" : intValueSO.Value.ToString();
    }

    private void IntValueSO_OnValueChanged()
    {
        ValueChanged.Invoke(this);
    }
}

