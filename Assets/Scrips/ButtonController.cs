using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public DispenserCube dispenser;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null && rb.linearVelocity.magnitude < 0.1f)
            {
                dispenser.DropCube();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        dispenser.ResetI();
    }
}
