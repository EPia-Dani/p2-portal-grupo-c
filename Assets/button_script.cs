using UnityEngine;
using UnityEngine.UI;

public class boton_script : MonoBehaviour
{
    public Surtidor_script surtidor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(surtidor.CrearCubo);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
