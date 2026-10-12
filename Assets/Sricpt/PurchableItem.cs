using UnityEngine;

public class PurchableItem : MonoBehaviour
{
    public string itemName = "";
    public int price = 2;
    public bool scanned;

    public void MarkScanned()
    {
        scanned = true;
        // optional: change colour, play a beep, etc.
    }

}
