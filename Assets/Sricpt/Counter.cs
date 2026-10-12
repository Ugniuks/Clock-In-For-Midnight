using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private Transform[] itemSpots;  // empty objects on the counter surface
    public int money;
    [SerializeField] private Transform[] queueSpots;  // [0] = in front of the register, then further back
    private List<Customer> line = new List<Customer>();

    public void Join(Customer c) => line.Add(c);
    public void Leave(Customer c) => line.Remove(c);
    public int PositionInLine(Customer c) => line.IndexOf(c);

    public IEnumerator Serve(List<GameObject> basket)
    {
        var spawned = new List<PurchableItem>();

        for (int i = 0; i < basket.Count && i < itemSpots.Length; i++)
        {
            GameObject obj = Instantiate(basket[i], itemSpots[i].position, itemSpots[i].rotation);
            spawned.Add(obj.GetComponent<PurchableItem>());
        }

        // wait until the player has scanned everything
        yield return new WaitUntil(() => spawned.TrueForAll(s => s.scanned));

        int total = 0;
        foreach (PurchableItem s in spawned)
        {
            total += s.price;
            Destroy(s.gameObject);
        }

        money += total;
        Debug.Log($"Paid {total}. Register: {money}");

        yield return new WaitForSeconds(0.5f);
    }

    public Vector3 GetSpot(int position)
    {
        if (queueSpots == null || queueSpots.Length == 0)
        {
            Debug.LogError("Counter has no Queue Spots assigned!", this);
            return transform.position;
        }

        position = Mathf.Clamp(position, 0, queueSpots.Length - 1);
        return queueSpots[position].position;
    }
}
