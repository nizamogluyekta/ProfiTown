using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;
    
    [Header("Boundaries")]
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;

    private Camera mainCamera;
    private Vector3 lastPanPosition;
    private float currentZoom;
    private bool isOrtho;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        isOrtho = mainCamera.orthographic;
        currentZoom = isOrtho ? mainCamera.orthographicSize : mainCamera.transform.position.y;
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

            // Get the magnitude of the vector between touch points
            float previousDistance = Vector2.Distance(
                touch1.position - touch1.deltaPosition,
                touch2.position - touch2.deltaPosition);
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);

            // Calculate the zoom magnitude based on the change in distance
            float delta = previousDistance - currentDistance;

            // Apply zoom
            if (isOrtho)
            {
                currentZoom = Mathf.Clamp(mainCamera.orthographicSize + delta * zoomSpeed, minZoom, maxZoom);
                mainCamera.orthographicSize = currentZoom;
            }
            else
            {
                Vector3 pos = transform.position;
                pos.y = Mathf.Clamp(pos.y + delta * zoomSpeed, minZoom, maxZoom);
                transform.position = pos;
                currentZoom = pos.y;
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
