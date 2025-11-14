using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingPortals : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private LayerMask portalSpawnMask;
    [SerializeField] private LayerMask invalidLayers;
    [SerializeField] private float range = 100f;
    [SerializeField] private GameObject bluePortalPrefab;
    [SerializeField] private GameObject orangePortalPrefab;
    private bool canShoot = true;
    private Camera playerCamera;
    private GameObject currentBluePortal;
    private GameObject currentOrangePortal;
    private Transform playerTransform;

    [Header("Placement")]
    [SerializeField] private string portalLimitTag = "PortalLimit";
    [SerializeField] private float surfaceEpsilon = 0.015f;
    [SerializeField] private float checkDepth = 0.35f;

    [Header("Resising")]
    [SerializeField] private GameObject portalBluePreview;    
    [SerializeField] private GameObject portalOrangePreview;   
    [SerializeField] private float minPortalScale = 0.5f;      
    [SerializeField] private float maxPortalScale = 2f;        
    [SerializeField] private float scrollScaleSpeed = 0.5f;
    private float currentBlueScale = 1f;
    private float currentOrangeScale = 1f;
    private bool isHolding = false;
    private GameObject currentPreviewInstance;
    private GameObject currentPreviewPrefab;

    
    public static event Action ShootingBluePortal;
    public static event Action ShootingOrangePortal;

    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        playerCamera = player.GetComponentInChildren<Camera>();
        if (player) playerTransform = player.transform;
    }

    private void Update()
    {
        if (!isHolding || currentPreviewPrefab == null || playerCamera == null)
            return;

        visualizePortal(currentPreviewPrefab);
        
    }

    private void ShootPortal(GameObject portalPrefab, ref GameObject currentPortal, bool blue, float scale = 1f)
    {
        if (!playerCamera || !portalPrefab || !canShoot) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, range, portalSpawnMask, QueryTriggerInteraction.Ignore))
            return;

        Vector3 wallNormal = hit.normal;
        Vector3 proposedPos = hit.point + wallNormal * surfaceEpsilon;
        Quaternion proposedRot = Quaternion.LookRotation(wallNormal, Vector3.up);

        if (!ValidatePoints(portalPrefab, hit, proposedPos, proposedRot))
            return;

        if (currentPortal != null)
            Destroy(currentPortal);

        currentPortal = Instantiate(portalPrefab, proposedPos, proposedRot);
        currentPortal.transform.localScale = Vector3.one * scale;

        InicilizePortal(currentPortal, hit.collider.gameObject);
        if (blue)
        {
            ShootingBluePortal?.Invoke();
        }
        else
        {
            ShootingOrangePortal?.Invoke();
        }
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

            Vector3 local = child.localPosition;
            local.z = 0f; 
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

    private void visualizePortal(GameObject portalPreviewPrefab)
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, range, portalSpawnMask, QueryTriggerInteraction.Ignore))
        {
            if (currentPreviewInstance != null)
                currentPreviewInstance.SetActive(false);
            return;
        }

        Vector3 wallNormal = hit.normal;
        Vector3 proposedPos = hit.point + wallNormal * surfaceEpsilon;
        Quaternion proposedRot = Quaternion.LookRotation(wallNormal, Vector3.up);

        GameObject portalRealPrefab = (portalPreviewPrefab == portalBluePreview) ? bluePortalPrefab : orangePortalPrefab;

        if (!ValidatePoints(portalRealPrefab, hit, proposedPos, proposedRot))
        {
            if (currentPreviewInstance != null)
                currentPreviewInstance.SetActive(false);
            return;
        }

        if (currentPreviewInstance == null || currentPreviewPrefab != portalPreviewPrefab)
        {
            if (currentPreviewInstance != null)
                Destroy(currentPreviewInstance);

            currentPreviewInstance = Instantiate(portalPreviewPrefab);
            currentPreviewPrefab = portalPreviewPrefab;
        }

        currentPreviewInstance.SetActive(true);
        currentPreviewInstance.transform.position = proposedPos;
        currentPreviewInstance.transform.rotation = proposedRot;

        float scroll = Mouse.current != null ? Mouse.current.scroll.ReadValue().y : 0f;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            if (portalPreviewPrefab == portalBluePreview)
            {
                currentBlueScale += scroll * scrollScaleSpeed * 0.01f;
                currentBlueScale = Mathf.Clamp(currentBlueScale, minPortalScale, maxPortalScale);
            }
            else
            {
                currentOrangeScale += scroll * scrollScaleSpeed * 0.01f;
                currentOrangeScale = Mathf.Clamp(currentOrangeScale, minPortalScale, maxPortalScale);
            }
        }

        float scaleToApply = (portalPreviewPrefab == portalBluePreview) ? currentBlueScale : currentOrangeScale;
        currentPreviewInstance.transform.localScale = Vector3.one * scaleToApply;
    }

    public void OnShootBlue(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;
            visualizePortal(portalBluePreview);
        }

        if (context.canceled)
        {
            isHolding = false;

            float scale = currentBlueScale;

            if (currentPreviewInstance != null)
            {
                Destroy(currentPreviewInstance);
                currentPreviewInstance = null;
                currentPreviewPrefab = null;
            }

            ShootPortal(bluePortalPrefab, ref currentBluePortal, true, scale);
        }
    }

    public void OnShootOrange(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isHolding = true;
            visualizePortal(portalOrangePreview);
        }

        if (context.canceled)
        {
            isHolding = false;

            float scale = currentOrangeScale;

            if (currentPreviewInstance != null)
            {
                Destroy(currentPreviewInstance);
                currentPreviewInstance = null;
                currentPreviewPrefab = null;
            }

            ShootPortal(orangePortalPrefab, ref currentOrangePortal, false, scale);
        }
    }
}



