using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform cam;

    private void LateUpdate()
    {
        if (cam == null)
        {
            if (Camera.main == null) return;
            cam = Camera.main.transform;
        }

        transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
    }
}
