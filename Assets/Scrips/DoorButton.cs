using UnityEngine;

public class DoorButton : MonoBehaviour
{


    public Transform leftDoor;
    public Transform rightDoor;
    public float openingAngle = 90f;
  

    private Quaternion InitialLeftRotation;
    private Quaternion InitialRotationRight;

    private void Start()
    {
        InitialLeftRotation = leftDoor.localRotation;
        InitialRotationRight = rightDoor.localRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            Debug.Log("Cube detected on button. Opening doors.");
            Debug.Log("Initial Left Door Rotation: " +  InitialLeftRotation * Quaternion.Euler(0, openingAngle, 0));
            Debug.Log("Initial Right Door Rotation: " + InitialRotationRight * Quaternion.Euler(0, -openingAngle, 0));
            leftDoor.localRotation = InitialLeftRotation * Quaternion.Euler(0, openingAngle, 0);
            rightDoor.localRotation = InitialRotationRight * Quaternion.Euler(0, -openingAngle, 0);
        }
    }
}
