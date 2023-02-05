using System;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

[Serializable]
public class StringValueSOVariable : IVariable, IVariableValueChanged
{
    public StringValueSO strignValueSO;

    public event Action<IVariable> ValueChanged;

    public object GetSourceValue(ISelectorInfo selector)
    {
        strignValueSO.OnValueChanged -= StrignValueSO_OnValueChanged;
        strignValueSO.OnValueChanged += StrignValueSO_OnValueChanged;
        return strignValueSO.IsNullWithErrorLog() ? "Null" : strignValueSO.Value;
    }

    private void StrignValueSO_OnValueChanged()
    {
        ValueChanged.Invoke(this);
    }
}

