using UnityEngine;

public class boton_portes : MonoBehaviour
{

    public Portes_abrir puerta; // Asigna la puerta que controla este botón
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cubo"))
        {
            puerta.Abrir();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cubo"))
        {
            puerta.Cerrar();
        }
    }
}
