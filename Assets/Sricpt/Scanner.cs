using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Scanner : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    [SerializeField] private GameObject beam;
    [SerializeField] private Transform scanOrigin;
    [SerializeField] private float scanRange = 1.5f;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {
        beam.SetActive(false);
        grabInteractable.activated.AddListener(Scan);
        grabInteractable.deactivated.AddListener(StopScan);
    }

    private void Scan(ActivateEventArgs args)
    {
        beam.SetActive(true);

        if (Physics.Raycast(scanOrigin.position, scanOrigin.forward, out RaycastHit hit, scanRange))
        {
            PurchableItem item = hit.collider.GetComponentInParent<PurchableItem>();
            if (item == null) return;

            if (item.scanned)
            {
                Debug.Log($"{item.itemName} already scanned");
                return;
            }

            item.MarkScanned();
            Debug.Log($"Scanned {item.itemName} ({item.price})");
        }
    }

    private void StopScan(DeactivateEventArgs args)
    {
        beam.SetActive(false);
    }

    private void OnDestroy()
    {
        grabInteractable.activated.RemoveListener(Scan);
        grabInteractable.deactivated.RemoveListener(StopScan);
    }
}
