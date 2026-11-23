using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SlidingWindow : MonoBehaviour
{
    public XRSimpleInteractable handle;
    public  Vector3 SlideDir = Vector3.right;
    public float maxSlideDistance = 1f;

    Transform hand;
    Vector3 startLocalPos;
    float grabOffset;

    void Awake()
    {
        startLocalPos = transform.localPosition;
        handle.selectEntered.AddListener(args =>
        {
            hand = args.interactorObject.transform;
            grabOffset = HandAlongAxis() - CurrentOffset();
        });
        handle.selectExited.AddListener(args => hand = null);
    }
    void Update()
    {
        if (hand == null) return;
        float offset = Mathf.Clamp(HandAlongAxis() - grabOffset, 0f, maxSlideDistance);
        transform.localPosition = startLocalPos + SlideDir.normalized * offset;
    }

    float HandAlongAxis()
    {
        Vector3 handLocal = transform.parent.InverseTransformPoint(hand.position);
        return Vector3.Dot(handLocal, SlideDir.normalized);
    }

    float CurrentOffset()
    {
        return Vector3.Dot(transform.localPosition - startLocalPos, SlideDir.normalized);
    }
}
