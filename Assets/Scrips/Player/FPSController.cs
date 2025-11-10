using UnityEngine;

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
    private bool IsGrounded;
    private bool isAiming;

    private void OnEnable()
    {
        PlayerInputHandler.MoveChanged += OnMoveChanged;
        PlayerInputHandler.LookDelta += OnLookDelta;
        PlayerInputHandler.SprintChanged += OnSprintChanged;
        PlayerInputHandler.JumpRequest += OnJumpRequested;
        //PlayerInputHandler.AimingChanged += OnAimingChanged;


    }
    private void OnDisable()
    {
        PlayerInputHandler.MoveChanged -= OnMoveChanged;
        PlayerInputHandler.LookDelta -= OnLookDelta;
        PlayerInputHandler.SprintChanged -= OnSprintChanged;
        PlayerInputHandler.JumpRequest -= OnJumpRequested;
        //PlayerInputHandler.AimingChanged -= OnAimingChanged;

    }
    void Start()
    {
        mYaw = transform.rotation.y; //Inicializa el valor del yaw con la rotacion inicial del jugador
        mPitch = mPitchController.localRotation.x; //Inicializa el valor del pitch y del yaw con la rotacion inicial del jugador
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; //Bloquea el cursor en el centro de la pantalla
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
            finalDirection *= sprintMultiplier; // Aplica el multiplicador de sprint
        }

        //JUMP
        mVerticalSpeed += Physics.gravity.y * Time.deltaTime; //Aplica la gravedad al vertical speed
        finalDirection.y = mVerticalSpeed * Time.deltaTime; //Aplica el vertical speed al movimiento final

        // Manejo de la gravedad y salto
        CollisionFlags collisionsFlags = controller.Move(finalDirection); //Aplica el movimiento al CharacterController y obtiene las colisiones
        IsGrounded = (collisionsFlags & CollisionFlags.CollidedBelow) != 0; //Comprueba si el CharacterController esta tocando el suelo
        if (IsGrounded && mVerticalSpeed > 0.0f)
        {
            mVerticalSpeed = 0.0f; // Fuerza hacia abajo para mantener el contacto con el suelo
        }
    }

    private void OnMoveChanged(Vector2 direction)
    {
        mDirection = direction;
    }
    private void OnLookDelta(Vector2 lookDelta)
    {
        mLookDirection = lookDelta;
    }
    private void OnSprintChanged(bool sprinting)
    {
        isSprinting = sprinting;
    }
    private void OnJumpRequested()
    {
        if (IsGrounded)
        {
            mVerticalSpeed = jumpSpeed;
            IsGrounded = false;
        }
    }
    private void OnAimingChanged(bool value)
    {
        isAiming = value;
    }
}
