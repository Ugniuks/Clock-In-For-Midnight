using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GasRegister : MonoBehaviour
{
    public TMP_Text totalText;         
    public int maxDigits = 3;

    public float expectedLiters;
    public string expectedGasName;

    public UnityEvent onCorrect;       
    public UnityEvent onWrong;

    string digits = "";
    Gas selectedGas;
    public Gas[] gases;

    public float totalMoney;
    bool orderActive;            

    void Start() => UpdateDisplay();

    public void AddDigit(int d)
    {
        if (digits.Length >= maxDigits) return;
        digits += d;
        UpdateDisplay();
    }

    public void Clear()
    {
        digits = "";
        selectedGas = null;
        UpdateDisplay();
    }

    public void SelectGas(Gas gas)
    {
        selectedGas = gas;
        UpdateDisplay();
    }

    public void SetOrder(float liters, string gasName)
    {
        expectedLiters = liters;
        expectedGasName = gasName;
        orderActive = true;
    }

    public void Pay()
    {
        if (!orderActive) return;                         
        if (digits == "" || selectedGas == null) return;   

        float liters = float.Parse(digits);
        float total = liters * selectedGas.price;
        bool correct = Mathf.Approximately(liters, expectedLiters)
                       && selectedGas.gasName == expectedGasName;

        Clear();
        orderActive = false;

        if (correct)
        {
            totalMoney += total;   
            onCorrect.Invoke();
        }
        else
        {
            onWrong.Invoke();
        }
    }

    void UpdateDisplay()
    {
        if (digits == "" || selectedGas == null)
        {
            totalText.text = digits == "" ? "Total Cost: x" : "Liters: " + digits;
            return;
        }
        float total = float.Parse(digits) * selectedGas.price;
        totalText.text = "Total Cost: " + total.ToString("0");
    }

}
