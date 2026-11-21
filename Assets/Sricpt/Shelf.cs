using UnityEngine;

public class Shelf : MonoBehaviour
{
    public GameObject itemPrefab;
    [SerializeField] private Transform[] itemSpots;
    private GameObject[] spawnedItems;

    private void Start()
    {
        Refill();
    }

    public void SetItem(GameObject newPrefab)
    {
        itemPrefab = newPrefab;
        Refill();
    }

    public void Refill()
    {
        if (spawnedItems != null)
        {
            foreach (GameObject item in spawnedItems)
            {
                if (item != null) Destroy(item);
            }
        }

        spawnedItems = new GameObject[itemSpots.Length];

        if (itemPrefab == null) return;

        for (int i = 0; i < itemSpots.Length; i++)
        {
            GameObject item = Instantiate(itemPrefab, itemSpots[i].position, itemSpots[i].rotation, itemSpots[i]);
            MakeDecorative(item);
            spawnedItems[i] = item;
        }
    }

    private void MakeDecorative(GameObject item)
    {
        // No collisions, so NPCs and the player can't bump or block on them
        foreach (Collider c in item.GetComponentsInChildren<Collider>())
            c.enabled = false;

        // Stop physics from moving the item
        foreach (Rigidbody rb in item.GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }
}