using UnityEngine;

public class Surtidor_script : MonoBehaviour
{
    [SerializeField] public GameObject cubPrefab;
    [SerializeField] public Transform puntoSurtidor;
    private int i = 1;

    public void SoltarCubo()
    {
        if(i != 1)
        {
            return;
        }
        Debug.Log("¡Soltando cubo!");
        Vector3 posicion = puntoSurtidor.position + Vector3.up * -1;
        Instantiate(cubPrefab, posicion, Quaternion.identity);
        i++;

    }
    public void ResetI ()
    {
        i = 1;
    }
}
