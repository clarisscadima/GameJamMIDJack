
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform playerCamera;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float airControl = 0.5f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float doubleJumpForce = 12f;
    [SerializeField] private float doubleTapTime = 0.3f;

    [Header("Suelo")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Cámara")]
    [SerializeField] private float mouseSensitivityX = 2f;
    [SerializeField] private float mouseSensitivityY = 2f;
    [SerializeField] private bool invertY = false;
    [SerializeField] private bool usarSuavizado = false;
    [SerializeField] private float suavizadoCamara = 10f;

    private bool isGrounded;
    private float lastJumpTime;
    private bool canDoubleJump;
    private float rotationX = 0f;
    private float rotationY = 0f;
    private float targetRotationX = 0f;
    private float targetRotationY = 0f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = groundCheckObj.transform;
        }

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>()?.transform;
            if (playerCamera == null)
                playerCamera = Camera.main?.transform;
        }

        SetupRigidbody();
        SetupCamera();
    }

    void SetupRigidbody()
    {
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SetupCamera()
    {
        if (playerCamera != null)
        {
            if (playerCamera.parent != transform)
                playerCamera.SetParent(transform);
            playerCamera.localPosition = new Vector3(0, 0.7f, 0);
            playerCamera.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        HandleCursor();
        HandleCamera();
        HandleJump();

        // Debug CONTINUO (sin necesidad de presionar F1)
        if (groundCheck != null)
        {
            bool groundDetected = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

            // Esto debería aparecer TODO EL TIEMPO en la consola
            Debug.Log($"Frame: {Time.frameCount} | Suelo: {groundDetected} | isGrounded: {isGrounded} | Y pos: {transform.position.y}");
        }
        else
        {
            Debug.LogError("GroundCheck es NULL!");
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
    }

    void HandleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void HandleCamera()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        targetRotationY += mouseX * mouseSensitivityX;
        targetRotationX += (invertY ? mouseY : -mouseY) * mouseSensitivityY;
        targetRotationX = Mathf.Clamp(targetRotationX, -90f, 90f);

        if (usarSuavizado)
        {
            rotationY = Mathf.Lerp(rotationY, targetRotationY, Time.deltaTime * suavizadoCamara);
            rotationX = Mathf.Lerp(rotationX, targetRotationX, Time.deltaTime * suavizadoCamara);
        }
        else
        {
            rotationY = targetRotationY;
            rotationX = targetRotationX;
        }

        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        if (playerCamera != null)
            playerCamera.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
            if (isGrounded) canDoubleJump = false;
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        direction.Normalize();

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
            currentSpeed *= sprintMultiplier;

        Vector3 targetVelocity = direction * currentSpeed;
        if (!isGrounded)
            targetVelocity *= airControl;

        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump(jumpForce, "Salto");
            }
            else if (Time.time - lastJumpTime < doubleTapTime && !canDoubleJump)
            {
                Jump(doubleJumpForce, "¡Doble salto!");
                canDoubleJump = true;
            }
            lastJumpTime = Time.time;
        }
    }

    void Jump(float force, string type)
    {
        rb.velocity = new Vector3(rb.velocity.x, force, rb.velocity.z);
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}