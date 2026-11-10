using UnityEngine;

/// <summary>Plain data describing how one NPC looks.</summary>
public struct AppearanceData
{
    public int hatIndex;     // -1 = bald (no hat)
    public Texture2D face;
    public Color bodyColor;
}

/// <summary>
/// Create via: Assets > Create > NPC > Appearance Factory
/// Holds the pools of options and rolls random appearances.
/// </summary>
[CreateAssetMenu(menuName = "NPC/Appearance Factory", fileName = "CustomerAppearanceFactory")]
public class CustomerAppearanceFactory : ScriptableObject
{
    [Header("Hats")]
    public GameObject[] hatPrefabs;
    [Range(0f, 1f)] public float baldChance = 0.1f; // chance of no hat at all

    [Header("Head")]
    public Texture2D[] faceTextures;

    [Header("Body color (HSV ranges)")]
    [Range(0f, 1f)] public float minSaturation = 0.4f;
    [Range(0f, 1f)] public float maxSaturation = 0.9f;
    [Range(0f, 1f)] public float minValue = 0.5f;
    [Range(0f, 1f)] public float maxValue = 1f;

    public AppearanceData Generate()
    {
        AppearanceData data = new AppearanceData();

        // Hat (bald is a valid outcome)
        bool hasHats = hatPrefabs != null && hatPrefabs.Length > 0;
        data.hatIndex = (!hasHats || Random.value < baldChance)
            ? -1
            : Random.Range(0, hatPrefabs.Length);

        // Face
        if (faceTextures != null && faceTextures.Length > 0)
            data.face = faceTextures[Random.Range(0, faceTextures.Length)];

        // Random body color
        data.bodyColor = Random.ColorHSV(
            0f, 1f,
            minSaturation, maxSaturation,
            minValue, maxValue);

        return data;
    }
}