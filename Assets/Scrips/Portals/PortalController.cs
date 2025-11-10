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
    [SerializeField] private float offsetNearPlane = 0.05f; 

    
    private float cooldownTimer = 0f;

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cooldownTimer <= 0f)
        {
            TeleportPlayer();
            cooldownTimer = reenterCooldown;
            if (mirrorPortal != null)
            {
                mirrorPortal.cooldownTimer = mirrorPortal.reenterCooldown;
            }
        }
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
    }

    private void LateUpdate()
    {
        if (!playerCamera || mirrorPortal == null || mirrorPortal.reflectionCamera == null) return;

        // Render del portal modo espejo 
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
}
