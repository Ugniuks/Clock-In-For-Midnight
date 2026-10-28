using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Scanner : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    [SerializeField] private GameObject beam;

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

    private void Scan(ActivateEventArgs args) => beam.SetActive(true);
    private void StopScan(DeactivateEventArgs args) => beam.SetActive(false);

    private void OnDestroy()
    {
        grabInteractable.activated.RemoveListener(Scan);
        grabInteractable.deactivated.RemoveListener(StopScan);
    }
}