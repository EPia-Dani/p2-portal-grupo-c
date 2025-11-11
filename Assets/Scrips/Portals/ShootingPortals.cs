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

    [Header("Debug")]
    [SerializeField] private bool verboseDebug = true;
    [SerializeField] private float drawRayTime = 1.0f;      // duración del Debug.DrawRay

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
        if (!playerCamera || !portalPrefab)
        {
            if (verboseDebug) Debug.LogWarning($"[Portals] Falta cámara ({playerCamera}) o prefab ({portalPrefab}).");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, range, portalSpawnMask, QueryTriggerInteraction.Ignore))
        {
            if (verboseDebug)
            {
                Debug.Log($"[Portals] SIN impacto en capas válidas. " +
                          $"range={range}, portalSpawnMask={portalSpawnMask.value}.");
            }
            return;
        }

        // Info del impacto
        if (verboseDebug)
        {
            string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);
            Debug.Log($"[Portals] Impacto OK → obj='{hit.collider.name}', layer={hit.collider.gameObject.layer}({layerName}), " +
                      $"point={hit.point:F3}, normal={hit.normal:F3}");
            Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.cyan, drawRayTime);
        }

        // Propuesta de pos/rot
        Vector3 wallNormal = hit.normal;
        Vector3 proposedPos = hit.point + wallNormal * surfaceEpsilon;
        Quaternion proposedRot = Quaternion.LookRotation(wallNormal, Vector3.up);

        if (verboseDebug)
        {
            Debug.Log($"[Portals] Propuesta: pos={proposedPos:F3}, rotForward={(proposedRot * Vector3.forward):F3}, " +
                      $"surfaceEpsilon={surfaceEpsilon}, checkDepth={checkDepth}");
        }

        // Validación con razón de fallo
        if (!ValidatePoints(portalPrefab, hit, proposedPos, proposedRot, out string reason))
        {
            if (verboseDebug) Debug.LogWarning($"[Portals] Colocación INVÁLIDA → {reason}");
            return;
        }

        Instantiate(portalPrefab, proposedPos, proposedRot);
        if (verboseDebug) Debug.Log("[Portals] Portal instanciado correctamente.");
    }

    private bool ValidatePoints(GameObject portalPrefab, RaycastHit hit, Vector3 portalPos, Quaternion portalRot, out string reason)
    {
        reason = "";
        Vector3 wallNormal = hit.normal;
        Vector3 intoWall = -wallNormal;

        // Recolectar puntos
        Transform[] trs = portalPrefab.GetComponentsInChildren<Transform>(true);
        int totalChildren = trs.Length;
        int limitCount = 0;
        foreach (var t in trs) if (t.CompareTag(portalLimitTag)) limitCount++;

        if (verboseDebug)
        {
            Debug.Log($"[Portals] Validando puntos: children={totalChildren}, PortalLimit={limitCount}, " +
                      $"spawnMask={portalSpawnMask.value}, invalidMask={invalidLayers.value}");
        }

        if (limitCount == 0)
        {
            reason = "El prefab no tiene hijos con tag 'PortalLimit'.";
            return false; // cámbialo a true si quieres permitirlo
        }

        int i = 0;
        foreach (Transform child in trs)
        {
            if (!child.CompareTag(portalLimitTag)) continue;
            i++;

            // Posición del punto en mundo usando la propuesta
            Vector3 pointWorld = portalPos + portalRot * child.localPosition;

            // Ray desde fuera hacia dentro de la pared
            Vector3 origin = pointWorld + wallNormal * surfaceEpsilon;
            Ray r = new Ray(origin, intoWall);

            // Dibujo el rayo para verlo en escena
            Debug.DrawRay(origin, intoWall * checkDepth, Color.green, drawRayTime);

            // 1) ¿Hay pared válida detrás?
            bool hasWall = Physics.Raycast(r, out RaycastHit wallHit, checkDepth, portalSpawnMask, QueryTriggerInteraction.Ignore);
            if (!hasWall)
            {
                reason = $"Punto#{i} '{child.name}': sin pared válida detrás (mask={portalSpawnMask.value}).";
                if (verboseDebug)
                {
                    Debug.Log($"[Portals] {reason} origin={origin:F3}, dir={intoWall:F3}, depth={checkDepth}");
                }
                return false;
            }
            else if (verboseDebug)
            {
                string wLayer = LayerMask.LayerToName(wallHit.collider.gameObject.layer);
                Debug.Log($"[Portals] Punto#{i} '{child.name}': pared OK → '{wallHit.collider.name}', " +
                          $"layer={wallHit.collider.gameObject.layer}({wLayer}), dist={wallHit.distance:F3}");
            }

            // 2) ¿Toca capas inválidas?
            bool hitsInvalid = Physics.Raycast(r, checkDepth, invalidLayers, QueryTriggerInteraction.Ignore);
            if (hitsInvalid)
            {
                reason = $"Punto#{i} '{child.name}': toca capa inválida (mask={invalidLayers.value}).";
                if (verboseDebug)
                {
                    Debug.DrawRay(origin, intoWall * checkDepth, Color.red, drawRayTime);
                    Debug.Log($"[Portals] {reason} origin={origin:F3}, dir={intoWall:F3}, depth={checkDepth}");
                }
                return false;
            }
        }

        return true;
    }
}


