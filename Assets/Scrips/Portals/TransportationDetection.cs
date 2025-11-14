using System;
using UnityEngine;

public class TransportationDetection : MonoBehaviour
{
    public static event Action<PortalController> TeleportationPlayer;
    public static event Action<PortalController, GameObject> TeleportationObject;
    [SerializeField] private PortalController portal;

    private static float nextTeleportTime = 0f;
    [SerializeField] private float teleportCooldown = 0.2f; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time < nextTeleportTime)
                return;
            nextTeleportTime = Time.time + teleportCooldown;

            TeleportationPlayer?.Invoke(portal);
        }
        else
        { 
            TeleportationObject?.Invoke(portal, other.gameObject);
        }
    }

        
}

