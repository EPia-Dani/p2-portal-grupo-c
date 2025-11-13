using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PortalController : MonoBehaviour
{
    [Header("Refs")]
    public Camera playerCamera;
    public PortalController mirrorPortal;
    public Camera reflectionCamera;
    [SerializeField] private float offsetNearPlane = 0.05f;    // Para clipping del render del portal

    [SerializeField] private GameObject portalSurface;         // Superficie de pared del portal (con Collider)

    private float cooldownTimer = 0f;

    private Collider wallCol;                        // collider de la pared (portalSurface)

    private void Awake()
    {
        var bc = GetComponent<BoxCollider>();
        bc.isTrigger = true;

        wallCol = portalSurface ? portalSurface.GetComponent<Collider>() : null;
    }

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        // “Quitar” la colisión de la pared: deshabilitar su collider
        wallCol = portalSurface ? portalSurface.GetComponent<Collider>() : wallCol;
        if (wallCol != null) wallCol.enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        ActiveCollaider();
    }

    private void LateUpdate()
    {
        if (!playerCamera || mirrorPortal == null || mirrorPortal.reflectionCamera == null) return;

        // Render del portal (tu mismo código)
        Vector3 camWorldPos = playerCamera.transform.position;
        Vector3 camLocalPos = transform.InverseTransformPoint(camWorldPos);
        camLocalPos.z = -camLocalPos.z;
        mirrorPortal.reflectionCamera.transform.position = mirrorPortal.transform.TransformPoint(camLocalPos);

        Vector3 camWorldFwd = playerCamera.transform.forward;
        Vector3 camLocalDir = transform.InverseTransformDirection(camWorldFwd);
        camLocalDir.z = -camLocalDir.z;
        mirrorPortal.reflectionCamera.transform.forward = mirrorPortal.transform.TransformDirection(camLocalDir);

        float distCamToThisPortal = Vector3.Distance(playerCamera.transform.position, transform.position);
        mirrorPortal.reflectionCamera.nearClipPlane = Mathf.Max(0f, distCamToThisPortal) + offsetNearPlane;
    }

    public void SetPortalSurface(GameObject surface)
    {
        portalSurface = surface;
        wallCol = portalSurface ? portalSurface.GetComponent<Collider>() : null;
    }
    public void ActiveCollaider()
    {
        wallCol.enabled = true;    
    }
}
