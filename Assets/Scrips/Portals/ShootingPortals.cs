using UnityEngine;

public class ShootingPortals : MonoBehaviour
{
    [Header("Basics")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask portalSpawnMask; // capas válidas para colocar
    [SerializeField] private LayerMask invalidLayers;   // capas que INVALIDAN
    [SerializeField] private float range = 100f;
    [SerializeField] private GameObject bluePortalPrefab;
    [SerializeField] private GameObject orangePortalPrefab;

    [Header("Placement")]
    [SerializeField] private string portalLimitTag = "PortalLimit";
    [SerializeField] private float surfaceEpsilon = 0.015f; // separa un pelín del muro
    [SerializeField] private float checkDepth = 0.35f;      // profundidad del ray hacia dentro

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
        if (isShooting) ShootPortal(bluePortalPrefab);
    }
    private void OnShootingOrangePortal(bool isShooting)
    {
        if (isShooting) ShootPortal(orangePortalPrefab);
    }

    private void ShootPortal(GameObject portalPrefab)
    {
        if (!playerCamera || !portalPrefab) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, range, portalSpawnMask, QueryTriggerInteraction.Ignore)) return;

        Vector3 wallNormal = hit.normal;
        Vector3 proposedPos = hit.point + wallNormal * surfaceEpsilon;
        Quaternion proposedRot = Quaternion.LookRotation(wallNormal, Vector3.up);

        if (!ValidatePoints(portalPrefab, hit, proposedPos, proposedRot)) return;

        Instantiate(portalPrefab, proposedPos, proposedRot);
    }

    private bool ValidatePoints(GameObject portalPrefab, RaycastHit hit, Vector3 portalPos, Quaternion portalRot)
    {
        Vector3 wallNormal = hit.normal;
        Vector3 intoWall = -wallNormal;

        Transform[] trs = portalPrefab.GetComponentsInChildren<Transform>(true);
        int limitCount = 0;
        foreach (var t in trs) if (t.CompareTag(portalLimitTag)) limitCount++;
        if (limitCount == 0) return false;

        foreach (Transform child in trs)
        {
            if (!child.CompareTag(portalLimitTag)) continue;

            // Si tus empties tienen Z local ≠ 0 y quedan detrás del plano del portal,
            // descomenta la siguiente línea:
            // var local = child.localPosition; local.z = 0f;

            Vector3 pointWorld = portalPos + portalRot * child.localPosition; // o *local si usas el ajuste de Z
            Vector3 origin = pointWorld + wallNormal * surfaceEpsilon;
            Ray r = new Ray(origin, intoWall);

            // Debe haber pared válida detrás
            if (!Physics.Raycast(r, out _, checkDepth, portalSpawnMask, QueryTriggerInteraction.Ignore))
                return false;

            // No debe tocar capas inválidas
            if (Physics.Raycast(r, checkDepth, invalidLayers, QueryTriggerInteraction.Ignore))
                return false;
        }

        return true;
    }
}

