using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;       // žaidėjo Transform
    public Vector3 offset = new Vector3(2f, 1.5f, -10f);  // kamera bus šiek tiek į priekį ir aukščiau

    [Header("Follow Smoothness")]
    [Range(0, 10)] public float smooth = 5f;   // kuo didesnis, tuo lėtesnis sekimas

    private void LateUpdate()
    {
        if (target == null) return;

        // Kamera juda švelniai link target pozicijos + offset
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.y = transform.position.y; // fiksuoja Y
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smooth * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
