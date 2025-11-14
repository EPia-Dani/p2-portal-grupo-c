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
        Transform entry = portal.transform;               // portal que atraviesas
        Transform exit = portal.mirrorPortal.transform;  // portal de salida

        // 1. Coords locales respecto al portal de entrada
        Vector3 localPos = entry.InverseTransformPoint(transform.position);
        Vector3 localDir = entry.InverseTransformDirection(transform.forward);

        // 2. Cruce del portal (invertir Z, por ejemplo)
        localPos.z = -localPos.z;
        localDir.z = -localDir.z;

        // 3. Transformar al sistema del portal de salida
        transform.position = exit.TransformPoint(localPos);
        transform.forward = exit.TransformDirection(localDir);

        float scaleFactor = exit.localScale.x / entry.localScale.x;
        transform.localScale *= scaleFactor;

        // 4. Pequeño avance para salir del trigger
        transform.position += transform.forward * 0.3f;   // tu exitOffset
    }
}
