using UnityEngine;

public class FloatBob : MonoBehaviour
{
    [SerializeField] private float height = 0.5f;      // base hover height
    [SerializeField] private float amplitude = 0.1f;   // how far it bobs up and down
    [SerializeField] private float speed = 2f;

    private float offset;

    private void Awake()
    {
        offset = Random.Range(0f, Mathf.PI * 2f);   // so customers don't all bob in sync
    }

    private void Update()
    {
        Vector3 pos = transform.localPosition;
        pos.y = height + Mathf.Sin(Time.time * speed + offset) * amplitude;
        transform.localPosition = pos;
    }
}
