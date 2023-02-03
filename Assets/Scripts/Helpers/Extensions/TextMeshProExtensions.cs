using TMPro;

public static class TextMeshProExtensions
{
    public static string GetTMPSpriteString(this TMP_SpriteAsset spriteAsset, int spriteIndex)
    {
        return $"<sprite=\"{spriteAsset.name}\" index={spriteIndex}>";
    }
}
