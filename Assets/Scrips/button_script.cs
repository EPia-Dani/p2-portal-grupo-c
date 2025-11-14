using UnityEngine;
using UnityEngine.UI;

public class boton_script : MonoBehaviour
{
    public Surtidor_script surtidor;


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null && rb.linearVelocity.magnitude < 0.1f)
            {
                Debug.Log("Cube");
                surtidor.SoltarCubo();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        surtidor.ResetI();
    }
}
