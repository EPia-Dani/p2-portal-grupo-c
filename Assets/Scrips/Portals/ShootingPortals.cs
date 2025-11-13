using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ShootingPortals : MonoBehaviour
{
    [Header("Basics")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask portalSpawnMask;
    [SerializeField] private LayerMask invalidLayers;
    [SerializeField] private float range = 100f;
    [SerializeField] private GameObject bluePortalPrefab;
    [SerializeField] private GameObject orangePortalPrefab;

    [Header("Placement")]
    [SerializeField] private string portalLimitTag = "PortalLimit";
    [SerializeField] private float surfaceEpsilon = 0.015f;
    [SerializeField] private float checkDepth = 0.35f;

    // Referencias al portal activo actual
    private GameObject currentBluePortal;
    private GameObject currentOrangePortal;
    private Transform playerTransform;



    private void Awake()
    {
        // cachea el player una vez
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) playerTransform = p.transform;
    }

    private void OnEnable()
    {
        PlayerInputHandler.ShootingBluePortal += OnShootingBluePortal;
        PlayerInputHandler.ShootingOrangePortal += OnShootingOrangePortal;
    }
    private void OnDisable()
    {
        PlayerInputHandler.ShootingBluePortal -= OnShootingBluePortal;
        PlayerInputHandler.ShootingOrangePortal -= OnShootingOrangePortal;
    }

    private void OnShootingBluePortal(bool isShooting)
    {
        if (isShooting)
        {
            ShootPortal(bluePortalPrefab, ref currentBluePortal);
        }
    }

    private void OnShootingOrangePortal(bool isShooting)
    {
        if (isShooting)
        {
            ShootPortal(orangePortalPrefab, ref currentOrangePortal);
        }
    }

    private void ShootPortal(GameObject portalPrefab, ref GameObject currentPortal)
    {
        if (!playerCamera || !portalPrefab) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, range, portalSpawnMask, QueryTriggerInteraction.Ignore))
            return;

        Vector3 wallNormal = hit.normal;
        Vector3 proposedPos = hit.point + wallNormal * surfaceEpsilon;
        Quaternion proposedRot = Quaternion.LookRotation(wallNormal, Vector3.up);

        if (!ValidatePoints(portalPrefab, hit, proposedPos, proposedRot))
            return;

        // Si ya hay un portal del mismo color, destrúyelo
        if (currentPortal != null)
            Destroy(currentPortal);

        // Instancia el nuevo portal y guarda la referencia
        currentPortal = Instantiate(portalPrefab, proposedPos, proposedRot);
        InicilizePortal(currentPortal, hit.collider.gameObject);

    }

    private bool ValidatePoints(GameObject portalPrefab, RaycastHit hit, Vector3 portalPos, Quaternion portalRot)
    {
        Vector3 wallNormal = hit.normal;
        Vector3 intoWall = - wallNormal;

        Transform[] trs = portalPrefab.GetComponentsInChildren<Transform>(true);
        int limitCount = 0;
        foreach (var t in trs) if (t.CompareTag(portalLimitTag)) limitCount++;
        if (limitCount == 0) return false;

        foreach (Transform child in trs)
        {
            if (!child.CompareTag(portalLimitTag)) continue;

            Vector3 local = child.localPosition;
            local.z = 0f; // aseguramos que los puntos estén en el plano del portal
            Vector3 pointWorld = portalPos + portalRot * local;

            Vector3 origin = pointWorld + wallNormal * surfaceEpsilon;
            Ray r = new Ray(origin, intoWall);

            if (!Physics.Raycast(r, out _, checkDepth, portalSpawnMask, QueryTriggerInteraction.Ignore))
                return false;

            if (Physics.Raycast(r, checkDepth, invalidLayers, QueryTriggerInteraction.Ignore))
                return false;
        }

        return true;
    }

    private void InicilizePortal(GameObject currentPortal, GameObject surface)
    {
        var pc = currentPortal.GetComponent<PortalController>();
        if (!pc) return;

        pc.playerCamera = playerCamera;

        pc.SetPortalSurface(surface);

        PortalController other = null;
        if (currentPortal == currentBluePortal && currentOrangePortal != null)
            other = currentOrangePortal.GetComponent<PortalController>();
        else if (currentPortal == currentOrangePortal && currentBluePortal != null)
            other = currentBluePortal.GetComponent<PortalController>();

        pc.mirrorPortal = other;
        if (other != null) other.mirrorPortal = pc;

        if (pc.reflectionCamera == null)
            pc.reflectionCamera = currentPortal.GetComponentInChildren<Camera>(true);
    }

}


