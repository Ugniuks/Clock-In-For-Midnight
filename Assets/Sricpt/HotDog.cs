using UnityEngine;

public class HotDog : MonoBehaviour
{
    public int price = 8;

    Bun bun;

    void Awake()
    {
        bun = GetComponentInChildren<Bun>();
    }

    public bool IsComplete => bun != null && bun.IsComplete;
}
