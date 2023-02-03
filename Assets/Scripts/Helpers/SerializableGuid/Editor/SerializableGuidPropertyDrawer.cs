using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using EditorHelpers;
using UnityEngine;
using UnityEngine.UIElements;



[CustomPropertyDrawer(typeof(SerializableGuid))]
public class SerializableGuidPropertyDrawer : PropertyDrawer
{
    private SerializedProperty serializedGuidByteArrayProp;
    private UnityEngine.Object unityObject;
    private bool isInited;
    private byte[] guidByteArray = new byte[16];
    private string guidString;
    private bool isValidGuid;
    private SerializedProperty currentProperty;
    private bool isPrefabOnDisc;
    private void Init(SerializedProperty property)
    {
        currentProperty = property;
        unityObject = property.serializedObject.targetObject;
        serializedGuidByteArrayProp = property.FindPropertyRelative("guidByteArray");
        
        isPrefabOnDisc = AssetDatabaseExtended.IsPrefabOnDisk(unityObject);

        if (isPrefabOnDisc)
        {
            //Undo.RecordObject(unityObject, "Set prefab guid to empty");
            Guid guid = EditorSerializationUtils.GetGuid(serializedGuidByteArrayProp, guidByteArray);
            if(guid != Guid.Empty)
            {
                EditorSerializationUtils.SetGuid(serializedGuidByteArrayProp, Guid.Empty);
            }
            //EditorSerializationUtils.SetGuid(serializedGuidByteArrayProp, Guid.Empty);
            guidString = Guid.Empty.ToString();
        }
        else
        {
            if (serializedGuidByteArrayProp.arraySize == 0)
            {
                Debug.Log(property.serializedObject.targetObject.name);
                property.SetValue(new SerializableGuid(Guid.Empty));
            }
            Guid guid = EditorSerializationUtils.GetGuid(serializedGuidByteArrayProp, guidByteArray);
            isValidGuid = guid != Guid.Empty && EditorGuidsGenerator.IsGuidTaken(guid, unityObject) == false;
            if (isValidGuid == false)
            {
                //guid = EditorGuidsGenerator.EnsureValidGuid(Guid.Empty, unityObject);
                //EditorSerializationUtils.SetGuid(serializedGuidByteArrayProp, guid);
                Debug.LogError($"Set new Guid for {unityObject.name}");
                //Debug.Log($"Is invalid guid on {unityObject.name}");
            }
            else
            {
                EditorGuidsGenerator.RegisterGuid(guid, unityObject);
            }
            guidString = guid.ToString();

        }
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        if (isInited == false || currentProperty != property)
        {
            Init(property);
            isInited = true;
        }
        float buttonWidth = 70;
        Rect guidLabelRect = rect;
        guidLabelRect.width -= buttonWidth;
        Rect buttonRect = rect;
        buttonRect.width = buttonWidth;
        buttonRect.x = rect.width - buttonWidth + 20;

       
        if (isPrefabOnDisc)
        {
            EditorGUI.HelpBox(rect, "Guid for prefab is not allowed", MessageType.Warning);
            return;
        }
        GUI.enabled = false;
        var prevGuiColor = GUI.color;
        if(isValidGuid == false)
        {
            GUI.color = Color.red;
        }
        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.TextField(guidLabelRect, guidString);
        GUI.enabled = true;
        if(isValidGuid == false)
        {
            if(GUI.Button(buttonRect, "Generate"))
            {
                var validGuid = EditorGuidsGenerator.EnsureValidGuid(Guid.Empty, unityObject);
                EditorSerializationUtils.SetGuid(serializedGuidByteArrayProp, validGuid);
            }
        }
        else if (GUI.Button(buttonRect, "Copy"))
        {
            EditorGUIUtility.systemCopyBuffer = guidString;
        }
       
        EditorGUI.indentLevel = 0;
        GUI.color = prevGuiColor;
    }

    
}
