using UnityEngine;

public class ObjectTransportation : MonoBehaviour
{
    private void OnEnable()
    {
        TransportationDetection.TeleportationObject += Transportation;
    }
    private void OnDisable()
    {
        TransportationDetection.TeleportationObject -= Transportation;
    }

    private void Transportation(PortalController portal)
    {
        Debug.Log("Objeto transportado a través del portal: " + portal.name);
        Transform entry = portal.transform;               
        Transform exit = portal.mirrorPortal.transform;  

        Vector3 localPos = entry.InverseTransformPoint(transform.position);
        Vector3 localDir = entry.InverseTransformDirection(transform.forward);

        localPos.z = -localPos.z;
        localDir.z = -localDir.z;

        transform.position = exit.TransformPoint(localPos);
        transform.forward = exit.TransformDirection(localDir);

        float scaleFactor = exit.localScale.x / entry.localScale.x;
        transform.localScale *= scaleFactor;

        transform.position += transform.forward * 0.3f;
    }
}
