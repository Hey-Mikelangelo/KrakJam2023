using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;

#if UNITY_EDITOR
public class EditorSavableVariable<T>
{
    private string saveKey;
    private T value;

    private static JsonSerializer jsonSerializer = new JsonSerializer()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented,
    };

    public EditorSavableVariable(string saveKey, T initialValue)
    {
        this.saveKey = saveKey;
        value = GetValueFromEditorPrefs(saveKey, initialValue);
    }
    private T GetValueFromEditorPrefs(string saveKey, T initialValue)
    {
        if (EditorPrefs.HasKey(saveKey) == false)
        {
            return initialValue;
        }
        string objectString = EditorPrefs.GetString(saveKey);
        if (objectString == "True" || objectString == "False")
        {
            objectString = objectString.ToLower();
        }
        JToken jToken = JToken.Parse(objectString);
        T obj = jToken.ToObject<T>(jsonSerializer);
        return obj;
    }
    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            this.value = value;
            string objectString = JToken.FromObject(value, jsonSerializer).ToString();
            EditorPrefs.SetString(saveKey, objectString);
        }

    }

    public void Save()
    {
        Value = value;
    }
}
#endif