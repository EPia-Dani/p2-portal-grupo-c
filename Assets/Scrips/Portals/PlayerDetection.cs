using System;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public static event System.Action<PortalController> TeleportationRequest;
    [SerializeField] private PortalController portal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportationRequest?.Invoke(portal);
        }
    }
}
