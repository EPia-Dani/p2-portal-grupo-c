using UnityEngine;

public class Surtidor_script : MonoBehaviour
{
    [SerializeField] public GameObject cubPrefab;
    [SerializeField] public Transform puntoSurtidor;


    public void SoltarCubo()
    {
        Debug.Log("¡Soltando cubo!");
        Vector3 posicion = puntoSurtidor.position + Vector3.up * -1;
        Instantiate(cubPrefab, posicion, Quaternion.identity);

    }
}
