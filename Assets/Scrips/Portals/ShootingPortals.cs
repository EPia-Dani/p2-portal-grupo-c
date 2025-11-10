using UnityEngine;

public class ShootingPortals : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask portalSpawnMask;
    [SerializeField] private float range = 100f;
    [SerializeField] private GameObject bluePortalPrefab;
    [SerializeField] private GameObject orangePortalPrefab;


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
            ShootPortal(bluePortalPrefab);
        }
    }
    private void OnShootingOrangePortal(bool isShooting)
    {
        if (isShooting)
        {
            ShootPortal(orangePortalPrefab);
        }
    }
    private void ShootPortal(GameObject portalPrefab)
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range, portalSpawnMask, QueryTriggerInteraction.Ignore))
        {
            // Instantiate or move the portal to the hit point
            Instantiate(portalPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}
