using System;
using UnityEngine;

public class TransportationDetection : MonoBehaviour
{
    public static event Action<PortalController> TeleportationPlayer;
    [SerializeField] private PortalController portal;

    // Cooldown global para todos los portales
    private static float nextTeleportTime = 0f;
    [SerializeField] private float teleportCooldown = 0.2f; // ajustable en el inspector

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Si aún estamos en cooldown, ignoramos este trigger
        if (Time.time < nextTeleportTime)
            return;

        // Marcamos el próximo instante en el que se permitirá teletransportar
        nextTeleportTime = Time.time + teleportCooldown;

        TeleportationPlayer?.Invoke(portal);
    }

    private void OnTriggerExit(Collider other)
    {
        // No toques nada del cooldown aquí
    }
}

