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

    private void Transportation(PortalController portal, GameObject gameObject)
    {
        if (gameObject != this.gameObject)
            return;
        Debug.Log("Objeto transportado a través del portal: " + portal.name);
        Transform entry = portal.transform;
        Transform exit = portal.mirrorPortal.transform;

        GameObject obj = Instantiate(gameObject);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.transform.localScale = transform.localScale;

        Destroy(gameObject); 

        Transform t = obj.transform;

        Vector3 localPos = entry.InverseTransformPoint(t.position);
        Vector3 localDir = entry.InverseTransformDirection(t.forward);

        localPos.z = -localPos.z;
        localDir.z = -localDir.z;

        t.position = exit.TransformPoint(localPos);
        t.forward = exit.TransformDirection(localDir);

        float scaleFactor = exit.localScale.x / entry.localScale.x;
        t.localScale *= scaleFactor;  

        t.position += t.forward * 0.3f;
    }

}
