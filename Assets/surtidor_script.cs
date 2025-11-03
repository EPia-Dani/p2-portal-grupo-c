using UnityEngine;

public class Surtidor_script : MonoBehaviour
{
    [SerializeField] public GameObject cubPrefab;
    [SerializeField] public Transform puntoSurtidor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CrearCubo()
    {
        Instantiate(cubPrefab, puntoSurtidor.position, Quaternion.identity);
    }
}
