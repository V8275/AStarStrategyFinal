using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private float zoomSpeed = 1f;
    [SerializeField]
    private float minDistance = 2f;
    [SerializeField]
    private float maxDistance = 10f;

    private CinemachineFollowZoom virtualCamera;
    private float currentDistance;

    void Start()
    {
        virtualCamera = FindFirstObjectByType<CinemachineFollowZoom>();
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scrollInput * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        virtualCamera.Width = currentDistance;
    }
}
