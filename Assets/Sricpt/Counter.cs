using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour
{
    [SerializeField] private Transform[] itemSpots;  // empty objects on the counter surface
    public int money;
    [SerializeField] private Transform[] queueSpots;  // [0] = in front of the register, then further back
    private List<Customer> line = new List<Customer>();

    public void Join(Customer c) => line.Add(c);
    public void Leave(Customer c) => line.Remove(c);
    public int PositionInLine(Customer c) => line.IndexOf(c);

    private bool payPressed;

    [SerializeField] private TMP_Text display;
    private int lastShown = -1;

    public void PressPayButton()
    {
        payPressed = true;
    }
    private void ShowText(string text)
    {
        if (display != null) display.text = text;
    }

    public IEnumerator Serve(List<GameObject> basket)
    {
        var spawned = new List<PurchableItem>();

        for (int i = 0; i < basket.Count && i < itemSpots.Length; i++)
        {
            GameObject obj = Instantiate(basket[i], itemSpots[i].position, itemSpots[i].rotation);
            spawned.Add(obj.GetComponent<PurchableItem>());
        }

        lastShown = -1;
        ShowText("Total: 0");

        // 1. scanning: update the display each time an item is scanned
        while (!spawned.TrueForAll(s => s.scanned))
        {
            UpdateRunningTotal(spawned);
            yield return null;
        }
        UpdateRunningTotal(spawned);

        // 2. all scanned: wait for the button
        ShowText($"Total: {lastShown}\nPress to pay");
        payPressed = false;
        yield return new WaitUntil(() => payPressed);

        int total = 0;
        foreach (PurchableItem s in spawned)
        {
            total += s.price;
            Destroy(s.gameObject);
        }

        money += total;
        ShowText("Paid!");
        yield return new WaitForSeconds(1f);
        ShowText("");                       // or "Welcome" while waiting for the next customer
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
    private void UpdateRunningTotal(List<PurchableItem> items)
    {
        int sum = 0;
        foreach (PurchableItem s in items)
            if (s.scanned) sum += s.price;

        if (sum == lastShown) return;       // only rewrite the text when it changes
        lastShown = sum;
        ShowText($"Total: {sum}");
    }
}
