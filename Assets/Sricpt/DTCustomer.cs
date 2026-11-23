using System.Collections;
using TMPro;
using UnityEngine;

public class DTCustomer : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private TMP_Text bubble;
    [SerializeField] private AudioSource voice;
    [SerializeField] private AudioClip speakSound;

    [Header("Order")]
    [SerializeField] private int minLiters = 5;
    [SerializeField] private int maxLiters = 60;

    void Say(string line)
    {
        Debug.Log($"Say -> bubble: {bubble}, voice: {voice}, clip: {speakSound}");
        if (bubble == null) return;

        bubble.text = line;
        bubble.gameObject.SetActive(true);

        if (voice != null && speakSound != null)
        {
            voice.pitch = Random.Range(0.9f, 1.2f);
            voice.PlayOneShot(speakSound);
        }
    }

    public void HideBubble()   
    {
        if (bubble != null) bubble.gameObject.SetActive(false);
    }

    public void StartOrder(GasRegister register)
    {
        int liters = Random.Range(minLiters, maxLiters + 1);
        Gas gas = register.gases[Random.Range(0, register.gases.Length)];

        register.SetOrder(liters, gas.gasName);   

        string[] lines =
        {
            $"{liters} liters of {gas.gasName}, please.",
            $"Hi! Give me {liters} liters of {gas.gasName}.",
            $"Fill me up with {liters} liters of {gas.gasName}."
        };
        Say(lines[Random.Range(0, lines.Length)]);
    }

    public void SayThanks()
    {
        string[] lines =
        {
            "Thank you! Have a great day!",
            "Perfect, thanks!",
            "Great, see you next time!"
        };
        Say(lines[Random.Range(0, lines.Length)]);
    }

    public void SayAngry()
    {
        string[] lines =
        {
            "That's not what I ordered! I'm leaving!",
            "Unbelievable! I'm never coming back here!",
            "Are you kidding me? Forget it!"
        };
        Say(lines[Random.Range(0, lines.Length)]);
    }

}
