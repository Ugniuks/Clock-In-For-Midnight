using UnityEngine;

public class HotDogZone : MonoBehaviour
{
    private HotDog current;

    public bool HasHotDog => current != null;
    public HotDog CurrentHotDog => current;

    private void OnTriggerEnter(Collider other)
    {
        HotDog h = other.GetComponentInParent<HotDog>();
        if(h != null)
        {
            current = h;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HotDog h = other.GetComponentInParent<HotDog>();
        if(h != null && h == current)
        {
            current = null;
        }
    }
}
