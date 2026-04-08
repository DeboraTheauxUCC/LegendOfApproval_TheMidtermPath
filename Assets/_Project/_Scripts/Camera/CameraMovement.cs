using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -6f);

    [Header("Follow")]
    [SerializeField] private float followSpeed = 15f;

    private Vector3 _smoothVelocity;

    private void FixedUpdate()
    {
        if (followTarget == null) return;

        Vector3 targetPosition = followTarget.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _smoothVelocity,
            1f / followSpeed);

        transform.LookAt(followTarget.position + Vector3.up * 1.5f);
    }
}