using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PortalController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;
    public PortalController mirrorPortal;
    public Camera reflectionCamera;

    [Header("Tuning")]
    [SerializeField] private float exitOffset = 0.3f;
    [SerializeField] private float reenterCooldown = 0.2f;

    // 👉 en vez de un bool, contamos colliders del player dentro
    private readonly HashSet<Collider> _playerParts = new HashSet<Collider>();
    private bool _cooldown;
    private float offsetNearPlane;

    void Awake()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;

        // 👉 haz el trigger “solo delante” y con grosor
        if (col.size.z < 0.2f) col.size = new Vector3(col.size.x, col.size.y, 0.2f);
        col.center = new Vector3(col.center.x, col.center.y, Mathf.Abs(col.size.z) * 0.5f);
    }

    void Start()
    {
        if (!playerCamera)
            playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera")?.GetComponent<Camera>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!BelongsToPlayer(other)) return;

        _playerParts.Add(other);

        // Según el PDF: teletransportar cuando el player entra en el collider del portal
        // + pequeño offset de salida para salir del trigger destino. :contentReference[oaicite:1]{index=1}
        if (!_cooldown)
        {
            TeleportPlayer();
            StartCoroutine(Cooldown());
            mirrorPortal.StartCoroutine(mirrorPortal.Cooldown());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!BelongsToPlayer(other)) return;
        _playerParts.Remove(other);
        // playerIsOverlapping = _playerParts.Count > 0;  // si lo necesitas para UI/debug
    }

    private bool BelongsToPlayer(Collider c)
    {
        // robusto: acepta colliders hijos del player
        return player && c.transform.root == player.root;
        // alternativamente: return c.transform.root.CompareTag("Player");
    }

    private void TeleportPlayer()
    {
        // 1) Local respecto al “portal virtual” (flip Z)
        Vector3 lPos = transform.InverseTransformPoint(player.position);
        lPos.z = -lPos.z;

        Vector3 lDir = transform.InverseTransformDirection(player.forward);
        lDir.z = -lDir.z;

        // 2) Transformar por el portal espejo
        player.position = mirrorPortal.transform.TransformPoint(lPos);
        player.forward = mirrorPortal.transform.TransformDirection(lDir);

        // 3) Empuje para salir del trigger destino (evita re-disparo)
        player.position += player.forward * exitOffset;  // :contentReference[oaicite:2]{index=2}
    }

    private System.Collections.IEnumerator Cooldown()
    {
        _cooldown = true;
        yield return new WaitForSeconds(reenterCooldown);
        _cooldown = false;
    }

    private void LateUpdate() 
    { 
        Vector3 worldPosition = playerCamera.transform.position;
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        localPosition.z = -localPosition.z;
        mirrorPortal.reflectionCamera.transform.position = mirrorPortal.transform.TransformPoint(localPosition);
        Vector3 worldDirection = playerCamera.transform.forward;
        Vector3 localDirection = transform.InverseTransformDirection(worldDirection);
        localDirection.z = -localDirection.z;
        mirrorPortal.reflectionCamera.transform.forward = mirrorPortal.transform.TransformDirection(localDirection);
        float distance = Vector3.Distance(mirrorPortal.reflectionCamera.transform.position, mirrorPortal.transform.position);
        mirrorPortal.reflectionCamera.nearClipPlane = Mathf.Max(0.0f, distance) + offsetNearPlane; 
    }
}
