using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GasPanelButton : MonoBehaviour
{
    public enum Kind { Digit, Gas, Pay }

    public Kind kind;
    public int digit;             
    public Gas gas;              
    public GasRegister panel;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Button")) return;

        switch (kind)
        {
            case Kind.Digit: panel.AddDigit(digit); break;
            case Kind.Gas: panel.SelectGas(gas); break;
            case Kind.Pay: panel.Pay(); break;
        }
    }
}
