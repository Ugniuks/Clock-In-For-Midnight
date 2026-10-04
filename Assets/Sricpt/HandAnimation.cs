using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimation : MonoBehaviour
{
    [SerializeField] private InputActionReference gripActionRefrence;
    [SerializeField] private InputActionReference triggerActionRefrence;

    private Animator animator;

    private string gripActionName = "Grip";
    private string triggerActionName = "Pinch";
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if(animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(animator == null)
        {
            return;
        }
        float gripValue = gripActionRefrence.action.ReadValue<float>();
        float triggerValue = triggerActionRefrence.action.ReadValue<float>();
        
            animator.SetFloat(gripActionName, gripValue);
            animator.SetFloat(triggerActionName, triggerValue);
        
    }
}
