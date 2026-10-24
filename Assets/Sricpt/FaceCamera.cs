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

        // TextMeshPro reads correctly when its forward points away from the viewer
        transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
    }
}
