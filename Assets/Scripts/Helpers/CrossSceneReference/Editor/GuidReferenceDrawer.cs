using System;
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(GuidReference), true)]
public class GuidReferenceDrawer : PropertyDrawer
{
    SerializedProperty sceneProp;
    SerializedProperty nameProp;
    SerializedProperty useSimpleReferenceProp;
    SerializedProperty componentReferenceProp;

    // cache off GUI content to avoid creating garbage every frame in editor
    GUIContent sceneLabel = new GUIContent("Containing Scene", "The target object is expected in this scene asset.");
    GUIContent clearButtonGUI = new GUIContent("Clear", "Remove Cross Scene Reference");
    GUIContent componentLabel = new GUIContent("Component");
    GUIContent cachedGameObjectNameLabel = new GUIContent("GameObject name");
    GUIContent notLoadedTooltip;
    SerializedProperty currentProperty;
    SerializedProperty serializableGuidProperty;
    GuidReference guidReference;
    Type componentType;
    Type guidReferenceType;
    bool isICrossSceneObject = false;
    bool isTypeConstrainedReference = false;
    bool isInited;
    byte[] guidBytesArray = new byte[16];
    (Guid guid, string stringValue) guidWithStringValue;
    private void DrawUseSimpleReferenceCheckbox()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(useSimpleReferenceProp);
        if (EditorGUI.EndChangeCheck())
        {
            ClearCache();
        }
    }

    private void Init(SerializedProperty property)
    {
        sceneProp = property.FindPropertyRelative("cachedScene");
        useSimpleReferenceProp = property.FindPropertyRelative("useSimpleReference");
        componentReferenceProp = property.FindPropertyRelative("componentReference");
        serializableGuidProperty = property.FindPropertyRelative("serializableGuid").FindPropertyRelative("guidByteArray");
        nameProp = property.FindPropertyRelative("cachedName");
        notLoadedTooltip = new GUIContent(nameProp.stringValue, "Component is not currently loaded.");
        guidReference = property.GetSerializedValue<GuidReference>();
        guidReferenceType = guidReference.GetType();
        
        if (guidReferenceType.IsSubclassOfRawGeneric(typeof(GuidReference<>)))
        {
            isTypeConstrainedReference = true;
            componentType = guidReferenceType.BaseType.GetGenericArguments()[0];
            if (componentType.IsSubclassOfRawGenericInterface(typeof(ICrossSceneComponent<>)))
            {
                isICrossSceneObject = true;
            }
        }
    }
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        if(isInited == false || currentProperty != property)
        {
            Init(property);
        }

        bool useSimpleReference = useSimpleReferenceProp.boolValue;
        
        if (isTypeConstrainedReference == false)
        {
            throw new Exception("Is not type constrained ref");
        }
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(rect, label, property);
        EditorGUI.LabelField(rect, label);
        float buttonWidth = 70;
        Rect objectRefFieldRect = rect;
        objectRefFieldRect.width -= buttonWidth;
        Rect buttonRect = rect;
        buttonRect.width = buttonWidth;
        buttonRect.x = rect.width - buttonWidth + 20;
        EditorGUI.indentLevel++;
        if (useSimpleReference)
        {
            EditorGUI.BeginChangeCheck();
            var componentReference = componentReferenceProp.objectReferenceValue;
            var component = EditorGUILayout.ObjectField(componentLabel, componentReference, componentType, true) as Component;
            if (EditorGUI.EndChangeCheck())
            {
                if (component == null)
                {
                    componentReferenceProp.objectReferenceValue = null;
                }
                else
                {
                    Component thisPropertyComponent = property.serializedObject.targetObject as Component;
                    if (thisPropertyComponent.gameObject.scene == component.gameObject.scene)
                    {
                        componentReferenceProp.objectReferenceValue = component;
                    }
                    else
                    {
                        Debug.LogError("Cannot add object from another scene. Toggle \'Use Simple Reference\'");
                    }
                }
            }
            DrawUseSimpleReferenceCheckbox();
        }
        else
        {
            Guid guid = EditorSerializationUtils.GetGuid(serializableGuidProperty, guidBytesArray);
            var component = ComponentsGuidManager.ResolveGuid(guid);
            bool isNotLoaded = guid != Guid.Empty && component == null;
            
            if (component != null)
            {
                if (isICrossSceneObject == false)
                {
                    component.TryGetComponent(componentType, out component);
                }
                GUI.enabled = false;
                
                EditorGUI.ObjectField(objectRefFieldRect, componentLabel, component, componentType, true);
                DrawClearButton(buttonRect);
                SetCachedCrossSceneValues(component);
                EditorGUILayout.ObjectField(sceneLabel, sceneProp.objectReferenceValue, typeof(SceneAsset), false);
                GUI.enabled = true;
                DrawUseSimpleReferenceCheckbox();
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                if (isNotLoaded)
                {
                    GUI.enabled = false;
                    string guidString;
                    if(guidWithStringValue.guid == guid)
                    {
                        guidString = guidWithStringValue.stringValue;
                    }
                    else
                    {
                        guidString = guid.ToString();
                        guidWithStringValue = (guid, guidString);
                    }
                    EditorGUILayout.LabelField(guidString);
                    Color prevColor = GUI.color;
                    GUI.color = Color.red;
                    EditorGUI.LabelField(objectRefFieldRect, cachedGameObjectNameLabel, notLoadedTooltip, EditorStyles.objectField);
                    GUI.color = prevColor;
                    DrawClearButton(buttonRect);

                    var sceneObject = sceneProp.objectReferenceValue;
                    if(sceneObject != null)
                    {
                        EditorGUILayout.ObjectField(sceneLabel, sceneObject, typeof(SceneAsset), false);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Scene reference is missing");
                    }
                    GUI.enabled = true;
                }
                else if (isICrossSceneObject)
                {
                    var crossSceneObject = EditorGUILayout.ObjectField(componentLabel, component, componentType, true) as ICrossSceneComponent;
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (crossSceneObject == null)
                        {
                            ClearValue();
                        }
                        else
                        {
                            EditorSerializationUtils.SetGuid(serializableGuidProperty, crossSceneObject.Guid);
                            SetCachedCrossSceneValues(crossSceneObject as Component);
                        }
                    }
                }
                else
                {
                    var assignedComponent = EditorGUILayout.ObjectField(componentLabel, component, componentType, true) as Component;
                    if (EditorGUI.EndChangeCheck())
                    {
                        if(assignedComponent == null)
                        {
                            ClearValue();
                        }
                        else if (assignedComponent.TryGetComponent(out GuidComponent guidComponent))
                        {
                            EditorSerializationUtils.SetGuid(serializableGuidProperty, guidComponent.Guid);
                            SetCachedCrossSceneValues(guidComponent);
                        }
                        else
                        {
                            Debug.LogError($"{assignedComponent.gameObject} does not have a {nameof(guidComponent)}");
                        }
                    }
                }

                DrawUseSimpleReferenceCheckbox();
            }
        }
        EditorGUI.indentLevel = 0;
        EditorGUI.EndProperty();

    }

    private void DrawClearButton(Rect rect)
    {
        var prevGuiEnabled = GUI.enabled;
        GUI.enabled = true;
        if (GUI.Button(rect, clearButtonGUI))
        {
            ClearValue();
        }
        GUI.enabled = prevGuiEnabled;
    }
    private void SetCachedCrossSceneValues(Component component)
    {
        if (component.IsNullWithErrorLog())
        {
            return;
        }
        var gameObject = component.gameObject;
        nameProp.stringValue = gameObject.name;
        sceneProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath<SceneAsset>(gameObject.scene.path);
    }

    private void ClearValue()
    {
        EditorSerializationUtils.SetGuid(serializableGuidProperty, Guid.Empty);
        ClearCache();
        nameProp.stringValue = null;
        sceneProp.objectReferenceValue = null;
    }

    private void ClearCache()
    {
        if (guidReference.Is_Not_NullWithErrorLog())
        {
            guidReference.ClearCache();
        }
    }
}

/*// Using a property drawer to allow any class to have a field of type GuidRefernce and still get good UX
// If you are writing your own inspector for a class that uses a GuidReference, drawing it with
// EditorLayout.PropertyField(prop) or similar will get this to show up automatically
[CustomPropertyDrawer(typeof(GuidReference), true)]
public class GuidReferenceDraawer : PropertyDrawer
{
    SerializedProperty guidProp;
    SerializedProperty sceneProp;
    SerializedProperty nameProp;
    SerializedProperty useSimpleReferenceProp;
    SerializedProperty componentReferenceProp;

    // cache off GUI content to avoid creating garbage every frame in editor
    GUIContent sceneLabel = new GUIContent("Containing Scene", "The target object is expected in this scene asset.");
    GUIContent clearButtonGUI = new GUIContent("Clear", "Remove Cross Scene Reference");

    // add an extra line to display source scene for targets
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        guidProp = property.FindPropertyRelative("serializedGuid");
        nameProp = property.FindPropertyRelative("cachedName");
        sceneProp = property.FindPropertyRelative("cachedScene");
        useSimpleReferenceProp = property.FindPropertyRelative("useSimpleReference");
        componentReferenceProp = property.FindPropertyRelative("componentReference");

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        bool isICrossSceneObject = false;
        bool isTypeConstrainedReference = false;
        Type componentType = null;
        var guidReference = property.GetSerializedValue<GuidReference>();
        var guidReferenceType = guidReference.GetType();
        if (guidReferenceType.IsSubclassOfRawGeneric(typeof(GuidReference<>)))
        {
            isTypeConstrainedReference = true;
            componentType = guidReferenceType.BaseType.GetGenericArguments()[0];
            if (componentType.IsSubclassOfRawGenericInterface(typeof(ICrossSceneObject<>)))
            {
                isICrossSceneObject = true;
            }
        }

        if (useSimpleReferenceProp.boolValue)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            if (isTypeConstrainedReference)
            {
                componentReferenceProp.objectReferenceValue =
                    EditorGUI.ObjectField(position, label, componentReferenceProp.objectReferenceValue, componentType, true);
            }
            else
            {
                EditorGUI.PropertyField(position, componentReferenceProp);
            }
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, useSimpleReferenceProp);
            EditorGUI.indentLevel--;
            return;
        }


        position.height = EditorGUIUtility.singleLineHeight;

        // Draw prefix label, returning the new rect we can draw in
        var guidCompPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        System.Guid currentGuid;
        GameObject currentGO = null;

        // working with array properties is a bit unwieldy
        // you have to get the property at each index manually
        byte[] byteArray = new byte[16];
        int arraySize = guidProp.arraySize;
        for (int i = 0; i < arraySize; ++i)
        {
            var byteProp = guidProp.GetArrayElementAtIndex(i);
            byteArray[i] = (byte)byteProp.intValue;
        }

        currentGuid = new System.Guid(byteArray);
        currentGO = GuidManager.ResolveGuid(currentGuid);
        GuidComponent currentGuidComponent = currentGO != null ? currentGO.GetComponent<GuidComponent>() : null;

        Component component = null;

        if (currentGuid != System.Guid.Empty && currentGuidComponent == null)
        {
            // if our reference is set, but the target isn't loaded, we display the target and the scene it is in, and provide a way to clear the reference
            float buttonWidth = 55.0f;

            guidCompPosition.xMax -= buttonWidth;

            bool guiEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.LabelField(guidCompPosition, new GUIContent(nameProp.stringValue, "Target GameObject is not currently loaded."), EditorStyles.objectField);
            GUI.enabled = guiEnabled;

            Rect clearButtonRect = new Rect(guidCompPosition);
            clearButtonRect.xMin = guidCompPosition.xMax;
            clearButtonRect.xMax += buttonWidth;

            if (GUI.Button(clearButtonRect, clearButtonGUI, EditorStyles.miniButton))
            {
                ClearPreviousGuid();
            }
        }
        else
        {
            if (isICrossSceneObject)
            {

            }
            // if our object is loaded, we can simply use an object field directly
            component = EditorGUI.ObjectField(guidCompPosition, currentGuidComponent, typeof(GuidComponent), true) as GuidComponent;
        }

        if (currentGuidComponent != null && component == null)
        {
            ClearPreviousGuid();
        }

        // if we have a valid reference, draw the scene name of the scene it lives in so users can find it
        if (component != null)
        {
            nameProp.stringValue = component.name;
            string scenePath = component.gameObject.scene.path;
            sceneProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            // only update the GUID Prop if something changed.   fixes multi-edit on GUID References
            Guid guid = default;
            if (component is GuidComponent guidComponent)
            {
                guid = guidComponent.Guid;
            }
            else if (component is ICrossSceneObject crossSceneObject)
            {

            }
            if (component != currentGuidComponent)
            {
                //byteArray = component.GetGuid().ToByteArray();
                arraySize = guidProp.arraySize;
                for (int i = 0; i < arraySize; ++i)
                {
                    var byteProp = guidProp.GetArrayElementAtIndex(i);
                    byteProp.intValue = byteArray[i];
                }
            }
        }

        EditorGUI.indentLevel++;
        position.y += EditorGUIUtility.singleLineHeight;
        bool cachedGUIState = GUI.enabled;
        GUI.enabled = false;
        EditorGUI.ObjectField(position, sceneLabel, sceneProp.objectReferenceValue, typeof(SceneAsset), false);
        GUI.enabled = cachedGUIState;
        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUILayout.PropertyField(useSimpleReferenceProp);
        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    void ClearPreviousGuid()
    {
        nameProp.stringValue = string.Empty;
        sceneProp.objectReferenceValue = null;

        int arraySize = guidProp.arraySize;
        for (int i = 0; i < arraySize; ++i)
        {
            var byteProp = guidProp.GetArrayElementAtIndex(i);
            byteProp.intValue = 0;
        }
    }
}*/
