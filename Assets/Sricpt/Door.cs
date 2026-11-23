using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent (typeof(BoxCollider))]
public class Door : MonoBehaviour
{
    public GameObject door;
    public Vector3 openRot = new Vector3(0, 0, 0);
    [SerializeField] private AudioClip bellSound;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Npc")
            Open();
    }

    void Open()
    {
        door.transform.localEulerAngles = openRot;
        AudioSource.PlayClipAtPoint(bellSound, transform.position, volume);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Npc")
            Close();
    }

    void Close()
    {
       door.transform.localEulerAngles -= openRot;
        //door.transform.localEulerAngles = new Vector3(90, 180, 0);
        //door.transform.localEulerAngles = Vector3.zero;
    }
}
