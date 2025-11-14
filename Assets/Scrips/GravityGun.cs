using UnityEngine;
using UnityEngine.InputSystem;

public class GravityGun : MonoBehaviour
{
    [Header("Gravity Gun")]
    public float distancia = 5f;
    public float fuerza = 700f;
    [SerializeField] Transform player;
    [SerializeField] Transform attachingPosition;
    [SerializeField] float attachingObjectSpeed = 10f;
    [SerializeField] float attachedRadius = 0.3f; 

    Rigidbody objectAttached;
    bool attachedObject = false;
    bool attachingObject = false;
    Quaternion attachingObjectStartRotation;


    void Update()
    {
        Vector3 grabOrigin = new Vector3(
            transform.position.x,
            transform.position.y - 0.6f,
            transform.position.z
        );

        Debug.DrawRay(grabOrigin, -transform.forward * distancia, Color.red);
    }

    void FixedUpdate()
    {
        UpdateAttachedObject();
    }

    void TryToCatch()
    {
        Vector3 grabOrigin = new Vector3(
            transform.position.x,
            transform.position.y - 0.6f,
            transform.position.z
        );

        RaycastHit hit;

        if (Physics.Raycast(grabOrigin, -transform.forward, out hit, distancia))
        {
            if (hit.collider.CompareTag("Cube") || hit.collider.CompareTag("Turret"))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null)
                {
                    objectAttached = rb;
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    rb.interpolation = RigidbodyInterpolation.Interpolate;
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                    attachedObject = false;
                    attachingObject = true;
                    attachingObjectStartRotation = rb.rotation;

                    if (hit.collider.CompareTag("Turret"))
                    {
                        rb.transform.rotation = player.rotation;
                    }
                }
            }
        }
    }

    void UpdateAttachedObject()
    {
        if (objectAttached == null) return;
        if (!attachingObject && !attachedObject) return;

        Vector3 euler = attachingPosition.rotation.eulerAngles;

        Vector3 desiredPos = attachingPosition.position;

        Vector3 origin = transform.position;
        Vector3 dirToDesired = desiredPos - origin;
        float distToDesired = dirToDesired.magnitude;
        dirToDesired /= Mathf.Max(distToDesired, 0.0001f);

        RaycastHit wallHit;
        Vector3 targetPos = desiredPos;

        if (Physics.Raycast(origin, dirToDesired, out wallHit, distToDesired))
        {
            if (wallHit.rigidbody != objectAttached)
            {
                targetPos = wallHit.point - dirToDesired * attachedRadius;
            }
        }

        if (!attachedObject)
        {
            Vector3 dir = targetPos - objectAttached.transform.position;
            float dist = dir.magnitude;
            float move = attachingObjectSpeed * Time.deltaTime;

            if (move >= dist)
            {
                attachedObject = true;
                attachingObject = false;

                objectAttached.MovePosition(targetPos);
                objectAttached.MoveRotation(Quaternion.Euler(0f, euler.y, euler.z));
            }
            else
            {
                dir /= Mathf.Max(dist, 0.0001f);

                objectAttached.MovePosition(
                    objectAttached.transform.position + dir * move
                );

                objectAttached.MoveRotation(
                    Quaternion.Lerp(
                        attachingObjectStartRotation,
                        Quaternion.Euler(0f, euler.y, euler.z),
                        1f - Mathf.Min(dist / 1.5f, 1f)
                    )
                );
            }
        }
        else
        {
            objectAttached.MovePosition(targetPos);
            objectAttached.MoveRotation(Quaternion.Euler(0f, euler.y, euler.z));
        }
    }

    void DropObject()
    {
        DropOrTrowObject(0f);
    }

    void ShootObject()
    {
        DropOrTrowObject(fuerza);
    }

    void DropOrTrowObject(float fuerzaLanzar)
    {
        if (objectAttached == null) return;

        objectAttached.isKinematic = false;
        objectAttached.useGravity = true;

        if (fuerzaLanzar != 0f)
        {
            objectAttached.AddForce(
                attachingPosition.forward * fuerzaLanzar,
                ForceMode.Impulse
            );
        }

        attachingObject = false;
        attachedObject = false;
        objectAttached = null;
    }
    public void OnGrabOrShoot(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (objectAttached == null)
            TryToCatch();
        else
            ShootObject();
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (objectAttached != null)
            DropObject();
    }
}
