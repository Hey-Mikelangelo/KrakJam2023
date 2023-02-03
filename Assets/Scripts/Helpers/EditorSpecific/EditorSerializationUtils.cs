using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class EditorSerializationUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetComponentValue<T>(this T objectToSetVal, string propertyName, object value) where T : UnityEngine.Object
    {
        Type typeRef = objectToSetVal.GetType();
        var field = typeRef.GetFieldDeep(propertyName);
        if (field == null)
        {
            throw new Exception($"Field {propertyName} not found");
        }
        field.SetValue(objectToSetVal, value);
        EditorUtility.SetDirty(objectToSetVal);
    }

    /// <summary>
    /// Get object private field value
    /// </summary>
    /// <typeparam name="T"> Type of object to get value from </typeparam>
    /// <typeparam name="W"> Type f field to get value of</typeparam>
    /// <param name="objectToGetValFrom">Object to get value from</param>
    /// <param name="propertyName">Name of the property</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static W GetComponentValue<T, W>(this T objectToGetValFrom, string propertyName)
    {
        Type typeRef = typeof(T);
        FieldInfo field = typeRef.GetField(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (W)field.GetValue(objectToGetValFrom);
    }

    public static T GetSerializedValue<T>(this SerializedProperty property)
    {
        object @object = property.serializedObject.targetObject;
        string[] propertyNames = property.propertyPath.Split('.');

        List<string> propertyNamesClean = new List<String>();

        for (int i = 0; i < propertyNames.Count(); i++)
        {
            if (propertyNames[i] == "Array")
            {
                if (i != (propertyNames.Count() - 1) && propertyNames[i + 1].StartsWith("data"))
                {
                    int pos = int.Parse(propertyNames[i + 1].Split('[', ']')[1]);
                    propertyNamesClean.Add($"-GetArray_{pos}");
                    i++;
                }
                else
                    propertyNamesClean.Add(propertyNames[i]);
            }
            else
                propertyNamesClean.Add(propertyNames[i]);
        }
        // Get the last object of the property path.
        foreach (string path in propertyNamesClean)
        {
            if (path.StartsWith("-GetArray"))
            {
                string[] split = path.Split('_');
                int index = int.Parse(split[split.Count() - 1]);
                IList l = (IList)@object;
                @object = l[index];
            }
            else
            {
                @object = @object.GetType()
                    .GetField(path, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .GetValue(@object);
            }
        }

        return (T)@object;
    }

    private const BindingFlags AllBindingFlags = (BindingFlags)(-1);

    /// <summary>
    /// Returns attributes of type <typeparamref name="TAttribute"/> on <paramref name="serializedProperty"/>.
    /// </summary>
    public static TAttribute[] GetAttributes<TAttribute>(this SerializedProperty serializedProperty, bool inherit)
        where TAttribute : Attribute
    {
        if (serializedProperty == null)
        {
            throw new ArgumentNullException(nameof(serializedProperty));
        }

        var targetObjectType = serializedProperty.serializedObject.targetObject.GetType();

        if (targetObjectType == null)
        {
            throw new ArgumentException($"Could not find the {nameof(targetObjectType)} of {nameof(serializedProperty)}");
        }

        foreach (var pathSegment in serializedProperty.propertyPath.Split('.'))
        {
            var fieldInfo = targetObjectType.GetField(pathSegment, AllBindingFlags);
            if (fieldInfo != null)
            {
                return (TAttribute[])fieldInfo.GetCustomAttributes<TAttribute>(inherit);
            }

            var propertyInfo = targetObjectType.GetProperty(pathSegment, AllBindingFlags);
            if (propertyInfo != null)
            {
                return (TAttribute[])propertyInfo.GetCustomAttributes<TAttribute>(inherit);
            }
        }

        throw new ArgumentException($"Could not find the field or property of {nameof(serializedProperty)}");
    }

    /// <summary>
    /// Returns attributes of type <typeparamref name="TAttribute"/> on <paramref name="serializedProperty"/>.
    /// </summary>
    public static Attribute[] GetAllAttributes(this SerializedProperty serializedProperty, bool inherit)
    {
        if (serializedProperty == null)
        {
            throw new ArgumentNullException(nameof(serializedProperty));
        }

        var targetObjectType = serializedProperty.serializedObject.targetObject.GetType();

        if (targetObjectType == null)
        {
            throw new ArgumentException($"Could not find the {nameof(targetObjectType)} of {nameof(serializedProperty)}");
        }

        foreach (var pathSegment in serializedProperty.propertyPath.Split('.'))
        {
            var fieldInfo = targetObjectType.GetField(pathSegment, AllBindingFlags);
            if (fieldInfo != null)
            {
                return (Attribute[])fieldInfo.GetCustomAttributes(inherit);
            }

            var propertyInfo = targetObjectType.GetProperty(pathSegment, AllBindingFlags);
            if (propertyInfo != null)
            {
                return (Attribute[])propertyInfo.GetCustomAttributes(inherit);
            }
        }

        throw new ArgumentException($"Could not find the field or property of {nameof(serializedProperty)}");
    }

    public static Guid GetGuid(SerializedProperty guidByteArrayProp, byte[] guidBytesArray)
    {
        int arraySize = guidByteArrayProp.arraySize;
        for (int i = 0; i < arraySize; ++i)
        {
            var byteProp = guidByteArrayProp.GetArrayElementAtIndex(i);
            guidBytesArray[i] = (byte)byteProp.intValue;
        }
        Guid guid;
        try
        {
            guid = new Guid(guidBytesArray);
        }
        catch
        {
            guid = Guid.Empty;
        }
        return guid;
    }

    public static void SetGuid(SerializedProperty guidByteArrayProp, Guid guid)
    {
        var guidBytesArray = guid.ToByteArray();
        var arraySize = guidBytesArray.Length;
        Undo.RecordObject(guidByteArrayProp.serializedObject.targetObject, "Set guid");
        for (int i = 0; i < arraySize; ++i)
        {
            var byteProp = guidByteArrayProp.GetArrayElementAtIndex(i);
            byteProp.intValue = guidBytesArray[i];
        }
        guidByteArrayProp.serializedObject.ApplyModifiedProperties();
    }

    public static FieldInfo GetFieldDeep(this Type type, string name)
    {
        bool found = false;
        do
        {
            var field = type.GetFields(BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(f => f.Name == name);
            if (field != default(FieldInfo))
            {
                return field;
            }
            else
            {
                type = type.BaseType;
            }
        } while (!found && type != null);

        return null;
    }

    public static bool TryFindPrivateBaseFieldValue<T>(this object obj, string name, out T value)
    {
        Type t = obj.GetType();
        bool found = false;
        value = default(T);
        do
        {
            var field = t.GetFields(BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(f => f.Name == name);
            if (field != default(FieldInfo))
            {
                value = (T)field.GetValue(obj);
                found = true;
            }
            else
            {
                t = t.BaseType;
            }
        } while (!found && t != null);

        return found;
    }

    /// (Extension) Get the value of the serialized property.
    public static object GetValue(this SerializedProperty property)
    {
        string propertyPath = property.propertyPath;
        object value = property.serializedObject.targetObject;
        int i = 0;
        while (NextPathComponent(propertyPath, ref i, out var token))
            value = GetPathComponentValue(value, token);
        return value;
    }

    public static void SetValueDirect(this SerializedProperty property, object value)
    {
        if (property == null)
            throw new System.NullReferenceException("SerializedProperty is null");

        object obj = property.serializedObject.targetObject;
        string propertyPath = property.propertyPath;
        var flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        var paths = propertyPath.Split('.');
        FieldInfo field = null;

        for (int i = 0; i < paths.Length; i++)
        {
            var path = paths[i];
            if (obj == null)
                throw new System.NullReferenceException("Can't set a value on a null instance");

            var type = obj.GetType();
            if (path == "Array")
            {
                path = paths[++i];
                var iter = (obj as System.Collections.IEnumerable);
                if (iter == null)
                    //Property path thinks this property was an enumerable, but isn't. property path can't be parsed
                    throw new System.ArgumentException("SerializedProperty.PropertyPath [" + propertyPath + "] thinks that [" + paths[i - 2] + "] is Enumerable.");

                var sind = path.Split('[', ']');
                int index = -1;

                if (sind == null || sind.Length < 2)
                    // the array string index is malformed. the property path can't be parsed
                    throw new System.FormatException("PropertyPath [" + propertyPath + "] is malformed");

                if (!Int32.TryParse(sind[1], out index))
                    //the array string index in the property path couldn't be parsed,
                    throw new System.FormatException("PropertyPath [" + propertyPath + "] is malformed");

                obj = iter.ElementAtOrDefault(index);
                continue;
            }

            field = type.GetField(path, flag);
            if (field == null)
                //field wasn't found
                throw new System.MissingFieldException("The field [" + path + "] in [" + propertyPath + "] could not be found");

            if (i < paths.Length - 1)
                obj = field.GetValue(obj);

        }

        var valueType = value.GetType();
        if (!valueType.Is(field.FieldType))
            // can't set value into field, types are incompatible
            throw new System.InvalidCastException("Cannot cast [" + valueType + "] into Field type [" + field.FieldType + "]");

        field.SetValue(obj, value);
    }
    public static System.Object ElementAtOrDefault(this System.Collections.IEnumerable collection, int index)
    {
        var enumerator = collection.GetEnumerator();
        int j = 0;
        for (; enumerator.MoveNext(); j++)
        {
            if (j == index) break;
        }

        System.Object element = (j == index)
            ? enumerator.Current
            : default(System.Object);

        var disposable = enumerator as System.IDisposable;
        if (disposable != null) disposable.Dispose();

        return element;
    }

    public static bool Is(this Type type, Type baseType)
    {
        if (type == null) return false;
        if (baseType == null) return false;

        return baseType.IsAssignableFrom(type);
    }

    public static bool Is<T>(this Type type)
    {
        if (type == null) return false;
        Type baseType = typeof(T);

        return baseType.IsAssignableFrom(type);
    }
    /// (Extension) Set the value of the serialized property.
    public static void SetValue(this SerializedProperty property, object value)
    {
        Undo.RecordObject(property.serializedObject.targetObject, $"Set {property.name}");

        SetValueNoRecord(property, value);

        EditorUtility.SetDirty(property.serializedObject.targetObject);
        property.serializedObject.ApplyModifiedProperties();
    }

    /// (Extension) Set the value of the serialized property, but do not record the change.
    /// The change will not be persisted unless you call SetDirty and ApplyModifiedProperties.
    public static void SetValueNoRecord(this SerializedProperty property, object value)
    {
        string propertyPath = property.propertyPath;
        object container = property.serializedObject.targetObject;

        int i = 0;
        NextPathComponent(propertyPath, ref i, out var deferredToken);
        while (NextPathComponent(propertyPath, ref i, out var token))
        {
            container = GetPathComponentValue(container, deferredToken);
            deferredToken = token;
        }
        Debug.Assert(!container.GetType().IsValueType, $"Cannot use SerializedObject.SetValue on a struct object, as the result will be set on a temporary.  Either change {container.GetType().Name} to a class, or use SetValue with a parent member.");
        SetPathComponentValue(container, deferredToken, value);
    }

    // Union type representing either a property name or array element index.  The element
    // index is valid only if propertyName is null.
    struct PropertyPathComponent
    {
        public string propertyName;
        public int elementIndex;
    }

    static Regex arrayElementRegex = new Regex(@"\GArray\.data\[(\d+)\]", RegexOptions.Compiled);

    // Parse the next path component from a SerializedProperty.propertyPath.  For simple field/property access,
    // this is just tokenizing on '.' and returning each field/property name.  Array/list access is via
    // the pseudo-property "Array.data[N]", so this method parses that and returns just the array/list index N.
    //
    // Call this method repeatedly to access all path components.  For example:
    //
    //      string propertyPath = "quests.Array.data[0].goal";
    //      int i = 0;
    //      NextPropertyPathToken(propertyPath, ref i, out var component);
    //          => component = { propertyName = "quests" };
    //      NextPropertyPathToken(propertyPath, ref i, out var component) 
    //          => component = { elementIndex = 0 };
    //      NextPropertyPathToken(propertyPath, ref i, out var component) 
    //          => component = { propertyName = "goal" };
    //      NextPropertyPathToken(propertyPath, ref i, out var component) 
    //          => returns false
    static bool NextPathComponent(string propertyPath, ref int index, out PropertyPathComponent component)
    {
        component = new PropertyPathComponent();

        if (index >= propertyPath.Length)
            return false;

        var arrayElementMatch = arrayElementRegex.Match(propertyPath, index);
        if (arrayElementMatch.Success)
        {
            index += arrayElementMatch.Length + 1; // Skip past next '.'
            component.elementIndex = int.Parse(arrayElementMatch.Groups[1].Value);
            return true;
        }

        int dot = propertyPath.IndexOf('.', index);
        if (dot == -1)
        {
            component.propertyName = propertyPath.Substring(index);
            index = propertyPath.Length;
        }
        else
        {
            component.propertyName = propertyPath.Substring(index, dot - index);
            index = dot + 1; // Skip past next '.'
        }

        return true;
    }

    static object GetPathComponentValue(object container, PropertyPathComponent component)
    {
        if (component.propertyName == null)
            return ((IList)container)[component.elementIndex];
        else
            return GetMemberValue(container, component.propertyName);
    }

    static void SetPathComponentValue(object container, PropertyPathComponent component, object value)
    {
        if (component.propertyName == null)
            ((IList)container)[component.elementIndex] = value;
        else
            SetMemberValue(container, component.propertyName, value);
    }

    static object GetMemberValue(object container, string name)
    {
        if (container == null)
            return null;
        var type = container.GetType();
        var field = type.GetFieldDeep(name);
        if(field != null)
        {
            return field.GetValue(container);
        }
        return null;
    }

    static void SetMemberValue(object container, string name, object value)
    {
        var type = container.GetType();
        var field = type.GetFieldDeep(name);
        if(field == null)
        {
            Debug.LogError($"Failed to set member {container}.{name} via reflection");
        }
        else
        {
            field.SetValue(container, value);
        }
    }
}
