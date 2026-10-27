using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Bun : MonoBehaviour
{
    [SerializeField] XRSocketInteractor socket;
    public bool IsComplete { get; private set; }

    void OnEnable() => socket.selectEntered.AddListener(OnSausagePlaced);
    void OnDisable() => socket.selectEntered.RemoveListener(OnSausagePlaced);

    void OnSausagePlaced(SelectEnterEventArgs args)
    {
        var sausage = args.interactableObject.transform.GetComponent<Sausage>();
        if (sausage == null || sausage.state != Sausage.State.Cooked) return;
        if (IsComplete) return;

        IsComplete = true;
        StartCoroutine(LockSausage(sausage));
    }

    IEnumerator LockSausage(Sausage sausage)
    {
        socket.enabled = false;
        sausage.GetComponent<XRGrabInteractable>().enabled = false;

        yield return null;

        var rb = sausage.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        sausage.GetComponent<CapsuleCollider>().enabled = false;

        Transform t = sausage.transform;
        t.SetParent(socket.attachTransform);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }
}
