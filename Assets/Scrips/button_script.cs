using UnityEngine;
using UnityEngine.UI;

public class boton_script : MonoBehaviour
{
    public Surtidor_script surtidor;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("detecto player");
            surtidor.SoltarCubo();
        }
    }
}
