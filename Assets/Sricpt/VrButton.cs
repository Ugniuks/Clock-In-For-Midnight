using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class VrButton : MonoBehaviour
{
    public float deadTime = 1.0f; // Time in seconds to wait before allowing another press
    private bool _deadactive = false; // Flag to indicate if the button is in dead time

    public UnityEvent onPressed, onReleased;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag== "Button" && !_deadactive)
        {
            onPressed?.Invoke();
            Debug.Log("Button Pressed");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag== "Button" && !_deadactive)
        {
            onReleased?.Invoke();
            Debug.Log("Button Released");
            StartCoroutine(DeadTimeCoroutine());
        }
    }

    private IEnumerator DeadTimeCoroutine()
    {
        _deadactive = true;
        yield return new WaitForSeconds(deadTime);
        _deadactive = false;
    }
}