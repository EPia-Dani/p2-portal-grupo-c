using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PortalController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform player;          // Root del jugador (donde vive el CC/Rigidbody)
    [SerializeField] private Camera playerCamera;       // Cámara del jugador (si no, se busca por tag)
    public PortalController mirrorPortal;               // Portal destino
    public Camera reflectionCamera;                     // Cámara “remota” del portal destino

    [Header("Tuning")]
    [SerializeField] private float exitOffset = 0.3f;   // Empuje para salir del trigger destino
    [SerializeField] private float reenterCooldown = 0.2f; // Cooldown sin corutinas (segundos)
    [SerializeField] private float offsetNearPlane = 0.05f; // Clip extra para la cámara remota

    // Control robusto de presencia del player dentro del trigger (varios colliders)
    private readonly HashSet<Collider> _playerParts = new HashSet<Collider>();

    // Cooldown sin IEnumerator
    private float cooldownTimer = 0f;

    private void Awake()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;

        // Trigger “con grosor” y un poco por delante del plano del portal
        if (col.size.z < 0.2f) col.size = new Vector3(col.size.x, col.size.y, 0.2f);
        col.center = new Vector3(col.center.x, col.center.y, Mathf.Abs(col.size.z) * 0.5f);
    }

    private void Start()
    {
        if (!playerCamera)
        {
            var go = GameObject.FindGameObjectWithTag("PlayerCamera");
            if (go) playerCamera = go.GetComponent<Camera>();
        }
    }

    private void Update()
    {
        // Cooldown sin corutinas
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!BelongsToPlayer(other)) return;

        _playerParts.Add(other);

        // Teletransportar al entrar, si no hay cooldown activo
        if (cooldownTimer <= 0f)
        {
            TeleportPlayer();

            // Activar cooldown local y sincronizar con el portal espejo
            cooldownTimer = reenterCooldown;
            if (mirrorPortal != null)
                mirrorPortal.cooldownTimer = mirrorPortal.reenterCooldown;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!BelongsToPlayer(other)) return;
        _playerParts.Remove(other);
    }

    private bool BelongsToPlayer(Collider c)
    {
        // Acepta colliders hijos del player (robusto)
        return player && c.transform.root == player.root;
        // Alternativa por tag:
        // return c.transform.root.CompareTag("Player");
    }

    private void TeleportPlayer()
    {
        if (player == null || mirrorPortal == null) return;

        // 1) Posición/dirección en local del “portal virtual” (flip Z)
        Vector3 lPos = transform.InverseTransformPoint(player.position);
        lPos.z = -lPos.z;

        Vector3 lDir = transform.InverseTransformDirection(player.forward);
        lDir.z = -lDir.z;

        // 2) Transformar por el portal espejo
        Vector3 targetPos = mirrorPortal.transform.TransformPoint(lPos);
        Vector3 targetFwd = mirrorPortal.transform.TransformDirection(lDir);

        // Componentes de movimiento del player
        var cc = player.GetComponent<CharacterController>();
        var rb = player.GetComponent<Rigidbody>();

        if (cc != null)
        {
            // Evitar que el CC deshaga el movimiento ese frame
            cc.enabled = false;
            player.SetPositionAndRotation(
                targetPos,
                Quaternion.LookRotation(targetFwd, player.up)
            );
            // Empuje de salida para no re-disparar el trigger destino
            player.position += targetFwd * exitOffset;
            Physics.SyncTransforms();
            cc.enabled = true;

            // Si tu controller usa yaw interno, actualízalo aquí (p.ej., mYaw = eulerAngles.y).
        }
        else if (rb != null)
        {
            // Teleport limpio con Rigidbody
            bool wasKinematic = rb.isKinematic;
            rb.isKinematic = true;

            player.SetPositionAndRotation(
                targetPos,
                Quaternion.LookRotation(targetFwd, player.up)
            );
            player.position += targetFwd * exitOffset;
            Physics.SyncTransforms();

            rb.isKinematic = wasKinematic;

            // Si el RB es dinámico, mapear velocidad a través del portal
            if (!rb.isKinematic)
            {
                Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
                localVel.z = -localVel.z;
                rb.linearVelocity = mirrorPortal.transform.TransformDirection(localVel);
            }
        }
        else
        {
            // Transform puro (sin CC ni RB)
            player.SetPositionAndRotation(
                targetPos,
                Quaternion.LookRotation(targetFwd, player.up)
            );
            player.position += targetFwd * exitOffset;
            Physics.SyncTransforms();
        }

        // Debug útil
        // Debug.Log($"Teleported to {player.position} facing {player.forward}");
    }

    private void LateUpdate()
    {
        if (!playerCamera || mirrorPortal == null || mirrorPortal.reflectionCamera == null) return;

        // ====== Render de la cámara remota (portal espejo) ======
        Vector3 camWorldPos = playerCamera.transform.position;
        Vector3 camLocalPos = transform.InverseTransformPoint(camWorldPos);
        camLocalPos.z = -camLocalPos.z;
        mirrorPortal.reflectionCamera.transform.position = mirrorPortal.transform.TransformPoint(camLocalPos);

        Vector3 camWorldFwd = playerCamera.transform.forward;
        Vector3 camLocalDir = transform.InverseTransformDirection(camWorldFwd);
        camLocalDir.z = -camLocalDir.z;
        mirrorPortal.reflectionCamera.transform.forward = mirrorPortal.transform.TransformDirection(camLocalDir);

        // nearClipPlane: distancia cámara jugador ↔ portal de entrada + offset
        float distCamToThisPortal = Vector3.Distance(playerCamera.transform.position, transform.position);
        mirrorPortal.reflectionCamera.nearClipPlane = Mathf.Max(0f, distCamToThisPortal) + offsetNearPlane;
    }
}
