using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public static class ScreenBlocking
{
    private static List<GraphicRaycaster> disabledGraphicRaycasters = new();
    private static PlayerController disabledPlayerController;
    private static bool prevIsInputActive;
    private static object disablingObject;
    private static readonly List<string> alwaysActiveMapsNames = new() { "Menu Interaction", "Tutorial"};
    private static List<InputActionMap> disabledActionMaps = new();

    private static List<InputAction> disabledActionsFromMenuActionMap = new();

    public static void DisableAllRootGraphicRaycasters(GraphicRaycaster excludeRaycaster = null)
    {
        var sceneCount = SceneManager.sceneCount;
        for (int sceneIndex = 0; sceneIndex < sceneCount; sceneIndex++)
        {
            var scene = SceneManager.GetSceneAt(sceneIndex);
            if (scene.IsValid() == false)
            {
                continue;
            }
            var rootGameObjects = scene.GetRootGameObjects();
            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                var rootGameObject = rootGameObjects[i];
                if (rootGameObject.TryGetComponent(out GraphicRaycaster graphicRaycaster))
                {
                    if (excludeRaycaster == graphicRaycaster)
                    {
                        continue;
                    }
                    if (graphicRaycaster.enabled == true)
                    {
                        graphicRaycaster.enabled = false;
                        disabledGraphicRaycasters.AddDistinct(graphicRaycaster);
                    }
                }
            }
        }
    }

    public static void EnableDisabledGraphicRaycasters()
    {
        int count = disabledGraphicRaycasters.Count;
        for (int i = 0; i < count; i++)
        {
            var raycaster = disabledGraphicRaycasters[i];
            if (raycaster.IsNull() == false)
            {
                raycaster.enabled = true;
            }
        }
        disabledGraphicRaycasters.Clear();
    }

    public static void DisablePlayerController(object obj)
    {
        if (disablingObject != null)
        {
            //Debug.Log("PLayer controller already disabled");
            return;
        }
        disabledPlayerController = PlayerController.ActiveController;
        prevIsInputActive = false;
        if (disabledPlayerController != null)
        {
            disablingObject = obj;
            prevIsInputActive = disabledPlayerController.IsInputActive;
            disabledPlayerController.SetInputActive(false);
            var playerInput = PlayerInput.GetPlayerByIndex(0);
            if (playerInput != null)
            {
                disabledActionMaps.Clear();
                var actionMaps = playerInput.actions.actionMaps;
                foreach (var map in actionMaps)
                {

                    if (map.enabled)
                    {
                        if (alwaysActiveMapsNames.Contains(map.name) == false)
                        {
                            map.Disable();
                            disabledActionMaps.Add(map);
                        }
                    }
                }
            }
        }
    }

    public static bool EnablePlayerController(object obj)
    {
        if (disablingObject != null && obj != disablingObject)
        {
            //Debug.Log("Not valid object");
            return false;
        }
        if (disabledPlayerController != null && prevIsInputActive)
        {
            disabledPlayerController.SetInputActive(true);
            foreach (var disabledMap in disabledActionMaps)
            {
                disabledMap.Enable();
            }
            foreach (var disabledAction in disabledActionsFromMenuActionMap)
            {
                disabledAction.Enable();
            }
            disabledActionMaps.Clear();
        }
        disabledPlayerController = null;
        disablingObject = null;
        return true;
    }
}
