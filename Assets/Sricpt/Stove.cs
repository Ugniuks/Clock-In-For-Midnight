using UnityEngine;

public class Stove : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var s = other.GetComponentInParent<Sausage>();
        if (s) s.SetOnStove(true);
    }

    void OnTriggerExit(Collider other)
    {
        var s = other.GetComponentInParent<Sausage>();
        if (s) s.SetOnStove(false);
    }
}
