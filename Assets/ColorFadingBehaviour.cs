using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image)), DisallowMultipleComponent]
public class ColorFadingBehaviour : MonoBehaviour
{
    [SerializeField] private Color clearColor;
    [SerializeField] private Color blockingColor;
    [SerializeField, ReadOnly] private Image fadeImage;
    public float FadeValue => Mathf.InverseLerp(0, 1, fadeImage.color.a);
    private void Reset()
    {
        fadeImage = GetComponent<Image>();
    }


    public void SetFadeValue01(float value)
    {
        if (fadeImage.IsNullWithErrorLog())
        {
            return;
        }
        fadeImage.color = Color.Lerp(clearColor, blockingColor, value);
    }
}
