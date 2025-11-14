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
        mYaw = transform.eulerAngles.y;                       // no uses rotation.y (cuaternión)
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
    private void Transportation(PortalController portal)
    {
        controller.enabled = false;
        Transform entry = portal.transform;               // portal que atraviesas
        Transform exit = portal.mirrorPortal.transform;  // portal de salida

        //Coords locales respecto al portal de entrada
        Vector3 localPos = entry.InverseTransformPoint(transform.position);
        Vector3 localDir = entry.InverseTransformDirection(transform.forward);

        //Cruce del portal (invertir Z, por ejemplo)
        localPos.z = -localPos.z;
        localDir.z = -localDir.z;

        // 3. Transformar al sistema del portal de salida
        transform.position = exit.TransformPoint(localPos);
        transform.forward = exit.TransformDirection(localDir);

        // Escalado proporcional
        float scaleFactor = exit.localScale.x / entry.localScale.x;
        transform.localScale *= scaleFactor;

        //Pequeño avance para salir del trigger
        transform.position += transform.forward * 0.3f;   // tu exitOffset

        //Sincronizar el controlador con la nueva rotación
        mYaw = transform.eulerAngles.y;
        controller.enabled = true;
        portal.mirrorPortal.ActiveCollaider();
    }

    void OnLook(InputValue value)
    {
        Vector2 pos = value.Get<Vector2>();//Obtiene el valor del input del raton
        mYaw += pos.x * rotationSpeed * Time.deltaTime;//Actualiza el valor del yaw
        mPitch += pos.y * rotationSpeed * Time.deltaTime;//Actualiza el valor del pitch (nota el signo positivo para movimiento invertido)
    }

    public void OnMove(InputAction.CallbackContext context)//Esto se hace con eventos del input sistem que es mas eficiente 
    {
        mDirection = context.ReadValue<Vector2>();//Obtiene el valor del input del teclado
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mLookDirection = context.ReadValue<Vector2>();//Obtiene el valor del input del raton

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
