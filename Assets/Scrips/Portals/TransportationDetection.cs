using System;
using UnityEngine;

public class TransportationDetection : MonoBehaviour
{
    public static event Action<PortalController> TeleportationPlayer;
    public static event Action<PortalController> TeleportationObject;
    [SerializeField] private PortalController portal;

    // Cooldown global para todos los portales
    private static float nextTeleportTime = 0f;
    [SerializeField] private float teleportCooldown = 0.2f; // ajustable en el inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time < nextTeleportTime)
                return;

            // Marcamos el próximo instante en el que se permitirá teletransportar
            nextTeleportTime = Time.time + teleportCooldown;

            TeleportationPlayer?.Invoke(portal);
        }
        else
        { 
            TeleportationObject?.Invoke(portal);
        }
    }

        
}

