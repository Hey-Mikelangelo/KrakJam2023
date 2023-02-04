using UnityEngine;

public static class LayersDatabase
{
    public static readonly string[] SolidLayersNames = new string[]
    {
        LayerNames.Default, LayerNames.Player, LayerNames.Interactable
    };

    public static readonly LayerMask InteractableLayerMask = 1 << LayerMask.NameToLayer(LayerNames.Interactable);
    public static readonly LayerMask SolidLayerMask = LayerMask.GetMask(SolidLayersNames);


}
