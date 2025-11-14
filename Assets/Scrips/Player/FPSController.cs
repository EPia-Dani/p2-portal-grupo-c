using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [Header("Iniciable variables")]
    [SerializeField] Transform mPitchController;
    [SerializeField] private Camera playerCamera;
    private CharacterController controller;

    [Header("Direction variables")]
    [SerializeField] float maxSpeed;
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] bool InvertPitch;
    [SerializeField] float maxPitch;
    [SerializeField] float minPitch;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpSpeed = 5.0f;

    private float mYaw;  // horizontal
    private float mPitch; // vertical

    private Vector2 mDirection;
    private Vector2 mLookDirection;
    private float mVerticalSpeed;
    private bool isSprinting;
    [SerializeField] private bool IsGrounded;

    private void OnEnable()
    {
        TransportationDetection.TeleportationPlayer += Transportation;
    }
    private void OnDisable()
    {
        TransportationDetection.TeleportationPlayer -= Transportation;
    }

    void Start()
    {
        mYaw = transform.eulerAngles.y;                    
        mPitch = mPitchController.localEulerAngles.x;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        mYaw += mLookDirection.x * rotationSpeed * Time.deltaTime;
        mPitch -= mLookDirection.y * rotationSpeed * Time.deltaTime;

        mPitch = Mathf.Clamp(mPitch, minPitch, maxPitch);
        transform.rotation = Quaternion.Euler(0.0f, mYaw, 0.0f);

        mPitchController.localRotation = Quaternion.Euler(mPitch * (InvertPitch ? -1 : 1), 0.0f, 0.0f);

        Vector3 finalDirection = (transform.forward * mDirection.y + transform.right * mDirection.x) * maxSpeed * Time.deltaTime;

        if (isSprinting)
        {
            finalDirection *= sprintMultiplier; 
        }

        mVerticalSpeed += Physics.gravity.y * Time.deltaTime; 
        finalDirection.y = mVerticalSpeed * Time.deltaTime; 
        // Manejo de la gravedad y salto
        CollisionFlags collisionsFlags = controller.Move(finalDirection);
        IsGrounded = (collisionsFlags & CollisionFlags.CollidedBelow) != 0; 
        if (IsGrounded && mVerticalSpeed > 0.0f)
        {
            mVerticalSpeed = 0.0f; 
        }
    }
    private void Transportation(PortalController portal)
    {
        controller.enabled = false;
        Transform entry = portal.transform;              
        Transform exit = portal.mirrorPortal.transform;  

        Vector3 localPos = entry.InverseTransformPoint(transform.position);
        Vector3 localDir = entry.InverseTransformDirection(transform.forward);

        localPos.z = -localPos.z;
        localDir.z = -localDir.z;

        transform.position = exit.TransformPoint(localPos);
        transform.forward = exit.TransformDirection(localDir);

        float scaleFactor = exit.localScale.x / entry.localScale.x;
        transform.localScale *= scaleFactor;

        transform.position += transform.forward * 0.3f;   

        mYaw = transform.eulerAngles.y;
        controller.enabled = true;
        portal.mirrorPortal.ActiveCollaider();
    }

    void OnLook(InputValue value)
    {
        Vector2 pos = value.Get<Vector2>();
        mYaw += pos.x * rotationSpeed * Time.deltaTime;
        mPitch += pos.y * rotationSpeed * Time.deltaTime;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        mDirection = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mLookDirection = context.ReadValue<Vector2>();

    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsGrounded)
        {
            mVerticalSpeed = jumpSpeed;
            IsGrounded = false;
        }
    }
}
