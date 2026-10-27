using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Sausage : MonoBehaviour
{
    public enum State { Raw, Cooked }
    public State state = State.Raw;

    [SerializeField] float cookTime = 8f;
    [SerializeField] Renderer rend;
    [SerializeField] Material rawMat, cookedMat;

    XRGrabInteractable grab;
    float cookProgress;
    bool onStove;


    void Awake()
    {
        rend = GetComponent<Renderer>();
        grab = GetComponent<XRGrabInteractable>();
    }
    public void SetOnStove(bool value) => onStove = value;

    void Update()
    {
        if (!onStove || state == State.Cooked) return;

        cookProgress += Time.deltaTime;

        if (cookProgress >= cookTime)
        {
            state = State.Cooked;
            rend.material = cookedMat;

            grab.interactionLayers = grab.interactionLayers.value | InteractionLayerMask.GetMask("HotDogSocket");
        }
    }

}
