using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 50f;
    
    [Header("Boundaries")]
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;

    private Camera mainCamera;
    private Vector3 lastPanPosition;
    private Vector2 lastZoomPosition1;
    private Vector2 lastZoomPosition2;
    private float initialZoomDistance;
    private float targetZoom;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        targetZoom = mainCamera.orthographicSize;
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        // Handle touch input
        if (Input.touchCount == 1) // Single touch - panning
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = GetWorldPosition(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 currentPosition = GetWorldPosition(touch.position);
                Vector3 delta = lastPanPosition - currentPosition;
                
                // Move the camera
                Vector3 newPosition = transform.position + delta;
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
                newPosition.y = transform.position.y; // Keep the same height
                
                transform.position = newPosition;
                lastPanPosition = GetWorldPosition(touch.position);
            }
        }
        else if (Input.touchCount == 2) // Two finger touch - zooming
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                lastZoomPosition1 = touch1.position;
                lastZoomPosition2 = touch2.position;
                initialZoomDistance = Vector2.Distance(touch1.position, touch2.position);
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float currentZoomDistance = Vector2.Distance(touch1.position, touch2.position);
                float deltaDistance = currentZoomDistance - initialZoomDistance;
                
                // Calculate new zoom
                targetZoom -= deltaDistance * zoomSpeed * Time.deltaTime;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
                mainCamera.orthographicSize = targetZoom;

                initialZoomDistance = currentZoomDistance;
            }
        }
    }

    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        
        if (groundPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        
        return Vector3.zero;
    }
}
