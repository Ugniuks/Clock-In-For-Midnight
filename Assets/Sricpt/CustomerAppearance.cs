using UnityEngine;

/// <summary>
/// Put this on the Customer prefab. Applies an AppearanceData to the model.
/// Safe to call repeatedly, so it works with object pooling.
/// </summary>
public class CustomerAppearance : MonoBehaviour
{
    [SerializeField] private Renderer headRenderer;
    [SerializeField] private Renderer bodyRenderer;
    [SerializeField] private Transform hatAnchor; // empty child on top of the head

    private GameObject[] hatInstances; // cached so pooled reuse doesn't allocate
    private GameObject activeHat;
    private Material headMat;
    private Material bodyMat;

    private void Awake()
    {
        // .material creates a per-NPC instance so changes don't affect other customers
        headMat = headRenderer.material;
        bodyMat = bodyRenderer.material;
    }

    public void Apply(AppearanceData data, CustomerAppearanceFactory factory)
    {
        // Body
        bodyMat.color = data.bodyColor;

        // Face
        if (data.face != null)
            headMat.mainTexture = data.face;

        // Hat
        if (activeHat != null)
            activeHat.SetActive(false);
        activeHat = null;

        if (data.hatIndex >= 0) // -1 = bald
        {
            if (hatInstances == null)
                hatInstances = new GameObject[factory.hatPrefabs.Length];

            if (hatInstances[data.hatIndex] == null)
            {
                hatInstances[data.hatIndex] = Instantiate(
                    factory.hatPrefabs[data.hatIndex], hatAnchor);
            }

            activeHat = hatInstances[data.hatIndex];
            activeHat.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        // Clean up the material instances created in Awake
        if (headMat != null) Destroy(headMat);
        if (bodyMat != null) Destroy(bodyMat);
    }
}