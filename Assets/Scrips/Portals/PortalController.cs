using UnityEngine;

public class PortalController : MonoBehaviour
{
    [Header("Refs")]
    public Camera playerCamera;
    public PortalController mirrorPortal;
    public Camera reflectionCamera;
    [SerializeField] private float offsetNearPlane = 0.05f;   
    [SerializeField] private GameObject portalSurface;         
    private float cooldownTimer = 0f;
    private Collider wallCol;                  

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
