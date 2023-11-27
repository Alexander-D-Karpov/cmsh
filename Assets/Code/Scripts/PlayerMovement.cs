using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5.0f;
    public float crouchSpeed = 2.5f;
    public float mouseSensitivity = 100.0f;
    public float crouchHeight = 0.5f;
    public float standHeight = 2.0f;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode uiToggleKey = KeyCode.Escape;

    private float xRotation = 0f; // Ensure this starts at 0 for forward facing
    private bool isInteractingWithUI = false;
    private bool isCrouching = false;

    private void Start()
    {
        ToggleCursorState(false);
        controller.height = standHeight;

        // Ensure the player starts with the camera facing forward
        Camera.main.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    private void Update()
    {
        // Check for UI interaction
        if (Input.GetKeyDown(uiToggleKey))
        {
            isInteractingWithUI = !isInteractingWithUI;
            ToggleCursorState(isInteractingWithUI);
        }

        // Check for crouching
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : standHeight;
        }

        // Only process movement and look if not interacting with UI
        if (!isInteractingWithUI)
        {
            ProcessMovement();
            ProcessLook();
        }
    }

    private void ProcessMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * (isCrouching ? crouchSpeed : speed) * Time.deltaTime);
    }

    private void ProcessLook()
    {
        // Check if we are running on a mobile device
        if (Application.isMobilePlatform)
        {
            // Check if there is at least one touch present
            if (Input.touchCount > 0)
            {
                // Get the first touch
                Touch touch = Input.GetTouch(0);

                // Process touch movement for rotation
                float touchX = touch.deltaPosition.x * mouseSensitivity * Time.deltaTime;
                float touchY = touch.deltaPosition.y * mouseSensitivity * Time.deltaTime;

                xRotation -= touchY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * touchX);
            }
        }
        else // Existing mouse look functionality
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }


    private void ToggleCursorState(bool isUIActive)
    {
        Cursor.lockState = isUIActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isUIActive;
        isInteractingWithUI = isUIActive;
    }

    public void TeleportAndLookAt(Vector3 position, Vector3 lookAtPoint)
    {
        controller.enabled = false; // Disable the controller before teleporting

        // Teleport the player to the position
        transform.position = position;

        // Calculate the direction to look at on the y-axis
        Vector3 direction = (lookAtPoint - position).normalized;
        direction.y = 0; // Keep the y-axis rotation as is

        // Create a rotation that looks in the direction on the y-axis
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Apply the rotation to the player's y-axis only
        transform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f);

        controller.enabled = true; // Re-enable the controller after teleporting
    }
}
