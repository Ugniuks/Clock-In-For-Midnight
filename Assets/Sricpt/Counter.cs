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

    [SerializeField] private HotDogZone hotdogZone;
    private bool hotdogPressed;


    public void PressPayButton()
    {
        payPressed = true;
    }
    public void PressHotdogButton()
    {
        hotdogPressed = true;
    }
    private void ShowText(string text)
    {
        if (display != null) display.text = text;
    }

    

    public IEnumerator Serve(List<GameObject> basket, bool wantsHotdog)
    {
        var spawned = new List<PurchableItem>();

        for (int i = 0; i < basket.Count && i < itemSpots.Length; i++)
        {
            GameObject obj = Instantiate(basket[i], itemSpots[i].position, itemSpots[i].rotation);
            spawned.Add(obj.GetComponent<PurchableItem>());
        }

        HotDog dog = null;          // set once the hotdog button has registered it
        hotdogPressed = false;
        payPressed = false;
        int lastKey = -1;

        while (true)
        {
            // hotdog: registered by the hotdog button, unregistered if it leaves the zone
            if (wantsHotdog)
            {
                if (hotdogPressed && hotdogZone.HasHotDog) dog = hotdogZone.CurrentHotDog;
                if (dog != null && hotdogZone.CurrentHotDog != dog) dog = null;
            }
            hotdogPressed = false;

            bool allScanned = spawned.TrueForAll(s => s.scanned);
            bool ready = allScanned && (!wantsHotdog || dog != null);

            if (payPressed && ready) break;
            payPressed = false;     // a press at the wrong moment is ignored

            // running total: scanned items + registered hotdog
            int sum = 0;
            foreach (PurchableItem s in spawned)
                if (s.scanned) sum += s.price;
            if (dog != null) sum += dog.price;

            // only rewrite the display when something changed
            int key = sum * 8 + (allScanned ? 4 : 0) + (dog != null ? 2 : 0) + (hotdogZone.HasHotDog ? 1 : 0);
            if (key != lastKey)
            {
                lastKey = key;

                string text = $"Total: {sum}";
                if (wantsHotdog && dog == null)
                    text += hotdogZone.HasHotDog ? "\nPress the hotdog button" : "\nWants: hotdog";
                else if (ready)
                    text += "\nPress to pay";

                ShowText(text);
            }

            yield return null;
        }

        int total = 0;
        foreach (PurchableItem s in spawned)
        {
            total += s.price;
            Destroy(s.gameObject);
        }
        if (dog != null)
        {
            total += dog.price;
            Destroy(dog.gameObject);
        }

        money += total;
        ShowText("Paid!");
        yield return new WaitForSeconds(1f);
        ShowText("");
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
