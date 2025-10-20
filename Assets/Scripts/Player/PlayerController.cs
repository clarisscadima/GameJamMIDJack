
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform playerCamera;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float airControl = 0.5f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float doubleJumpForce = 12f;
    [SerializeField] private float doubleTapTime = 0.3f;

    [Header("Suelo")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Cámara")]
    [SerializeField] private float mouseSensitivity = 2f;

    // Estados
    private bool isGrounded;
    private float lastJumpTime;
    private bool canDoubleJump;
    private float cameraRotationX;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        // Buscar GroundCheck automáticamente
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                GameObject groundCheckObj = new GameObject("GroundCheck");
                groundCheckObj.transform.SetParent(transform);
                groundCheckObj.transform.localPosition = new Vector3(0, -0.9f, 0);
                groundCheck = groundCheckObj.transform;
                Debug.Log("GroundCheck creado automáticamente");
            }
        }

        // Buscar cámara automáticamente
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>()?.transform;
            if (playerCamera == null)
            {
                playerCamera = Camera.main?.transform;
            }
        }

        SetupRigidbody();
        SetupCamera();
    }

    void SetupRigidbody()
    {
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void SetupCamera()
    {
        if (playerCamera != null)
        {
            playerCamera.SetParent(transform);
            playerCamera.localPosition = new Vector3(0, 0.7f, 0);
        }
    }

    void Update()
    {
        HandleJump();
        HandleCamera();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
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

        Vector3 direction = (transform.forward * vertical + transform.right * horizontal).normalized;
        Vector3 moveVelocity = direction * moveSpeed;

        if (!isGrounded) moveVelocity *= airControl;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Salto normal en suelo
            if (isGrounded)
            {
                Jump(jumpForce, "Salto normal");
            }
            // Salto doble en aire (doble tap)
            else if (Time.time - lastJumpTime < doubleTapTime && !isGrounded)
            {
                Jump(doubleJumpForce, "SALTO DOBLE!");
                canDoubleJump = false;
            }
            // Primer salto en aire
            else if (!isGrounded && !canDoubleJump)
            {
                Jump(jumpForce, "Salto en aire");
                canDoubleJump = true;
            }

            lastJumpTime = Time.time;
        }
    }

    void Jump(float force, string type)
    {
        rb.velocity = new Vector3(rb.velocity.x, force, rb.velocity.z);
        Debug.Log(type);
    }

    void HandleCamera()
    {
        // Rotación horizontal (player)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);

        // Rotación vertical (cámara)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        if (playerCamera != null)
            playerCamera.localEulerAngles = new Vector3(cameraRotationX, 0, 0);
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