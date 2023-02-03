using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerNames 
{
    public const string Default = "Default";
    public const string TransoarentFX = "TransoarentFX";
    public const string IgnoreRaycast = "Ignore Raycast";
    public const string Player = "Player";
    public const string Water = "Water";
    public const string UI = "UI";
    public const string Interactable = "Interactable";
    public const string Grid = "Grid";
    public const string ActivatingCollider = "ActivatingCollider";
}

public static class LayersDatabase
{
    public static readonly string[] SolidLayersNames = new string[]
    {
        LayerNames.Default, LayerNames.Player, LayerNames.Interactable
    };

    public static readonly LayerMask InteractableLayerMask = 1 << LayerMask.NameToLayer(LayerNames.Interactable);
    public static readonly LayerMask ActivatingColliderLayerMask = 1 << LayerMask.NameToLayer(LayerNames.ActivatingCollider);
    public static readonly LayerMask SolidLayers = LayerMask.GetMask(SolidLayersNames);


}
