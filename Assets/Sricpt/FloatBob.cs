using UnityEngine;

public class FloatBob : MonoBehaviour
{
    [SerializeField] private float height = 0.5f;      
    [SerializeField] private float amplitude = 0.1f;  
    [SerializeField] private float speed = 2f;

    private float offset;

    private void Awake()
    {
        offset = Random.Range(0f, Mathf.PI * 2f);   
    }

    private void Update()
    {
        Vector3 pos = transform.localPosition;
        pos.y = height + Mathf.Sin(Time.time * speed + offset) * amplitude;
        transform.localPosition = pos;
    }
}
