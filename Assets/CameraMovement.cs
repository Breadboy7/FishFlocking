using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed of camera movement
    public float lookSpeedX = 1f; // Horizontal mouse sensitivity
    public float lookSpeedY = 1f; // Vertical mouse sensitivity
    public float minZoom = 2f; // Minimum zoom distance
    public float maxZoom = 20f; // Maximum zoom distance
    public float zoomSpeed = 10f; // Speed of zooming
    public float verticalSpeed = 5f; // Speed for moving up/down
    public float rotationSmoothness = 10f; // Smoothness of camera rotation

    private float currentZoom = 10f; // Current zoom distance
    private float rotationX = 0f; // Current rotation around the X axis (vertical)
    private float rotationY = 0f; // Current rotation around the Y axis (horizontal)

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle free movement with WASD, Space, and Ctrl
        MoveCamera();

        // Handle mouse rotation to look around
        RotateCamera();

        // Handle zooming in and out with the scroll wheel
        ZoomCamera();
    }

    // Move the camera based on WASD, Space, and Ctrl
    void MoveCamera()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys
        float vertical = Input.GetAxis("Vertical"); // W/S or Up/Down arrow keys

        // Vertical movement using Space and Ctrl
        float upward = 0f;
        if (Input.GetKey(KeyCode.Space)) upward = 1f; // Move up
        if (Input.GetKey(KeyCode.LeftControl)) upward = -1f; // Move down

        // Calculate movement vector
        Vector3 move = (transform.right * horizontal + transform.forward * vertical + Vector3.up * upward);
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    // Rotate the camera based on mouse movement
    void RotateCamera()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY; // Vertical mouse movement
        rotationY += Input.GetAxis("Mouse X") * lookSpeedX; // Horizontal mouse movement

        // Clamp the vertical rotation to avoid flipping
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);

        // Apply smooth rotation
        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
    }

    // Zoom the camera in and out using the scroll wheel
    void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // Get scroll input
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom); // Clamp the zoom to a set range

        // Adjust the camera position based on zoom level
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, currentZoom, Time.deltaTime * zoomSpeed);
    }
}
