using UnityEngine;

public class boton_portes : MonoBehaviour
{


    public Transform puertaIzquierda;
    public Transform puertaDerecha;
    public float anguloApertura = 90f;
  

    private Quaternion rotacionInicialIzquierda;
    private Quaternion rotacionInicialDerecha;

    private void Start()
    {
        // rot init
        rotacionInicialIzquierda = puertaIzquierda.localRotation;
        rotacionInicialDerecha = puertaDerecha.localRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hola");
            
            puertaIzquierda.localRotation = rotacionInicialIzquierda * Quaternion.Euler(0, anguloApertura, 0);
            puertaDerecha.localRotation = rotacionInicialDerecha * Quaternion.Euler(0, -anguloApertura, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            puertaIzquierda.localRotation = rotacionInicialIzquierda;
            puertaDerecha.localRotation = rotacionInicialDerecha;
        }
    }




}
