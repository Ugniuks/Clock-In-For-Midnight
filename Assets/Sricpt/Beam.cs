using UnityEngine;

public class Beam : MonoBehaviour
{
    [SerializeField] private AudioClip scanSound;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private void OnTriggerStay(Collider other)
    {
        PurchableItem item = other.GetComponentInParent<PurchableItem>();
        if (item == null || item.scanned) return;

        item.MarkScanned();
        AudioSource.PlayClipAtPoint(scanSound, transform.position, volume);
        Debug.Log($"Scanned {item.itemName} ({item.price})");
    }
}