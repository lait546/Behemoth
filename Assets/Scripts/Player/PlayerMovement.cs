using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    [Header("Camera")]
    public Transform cameraPivot;
    public Camera playerCamera;

    private Rigidbody rb;

    private float verticalLookRotation;
    private const float minVerticalAngle = -60f;
    private const float maxVerticalAngle = 60f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (playerCamera == null)
            playerCamera = Camera.main;

        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (Time.timeScale > 0f)
            HandleRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();

        if (rb.angularVelocity != Vector3.zero)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * vertical + right * horizontal).normalized;

        Vector3 newVelocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
        rb.velocity = newVelocity;
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minVerticalAngle, maxVerticalAngle);

        if (cameraPivot != null)
        {
            cameraPivot.localEulerAngles = Vector3.right * verticalLookRotation;
        }
        else
        {
            playerCamera.transform.localEulerAngles = Vector3.right * verticalLookRotation;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public Camera GetPlayerCamera()
    {
        return playerCamera;
    }

    public Transform GetCameraPivot()
    {
        return cameraPivot;
    }
}