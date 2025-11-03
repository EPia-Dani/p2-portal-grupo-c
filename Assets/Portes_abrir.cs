using UnityEngine;

public class Portes_abrir : MonoBehaviour
{
    public Vector3 angulo = new Vector3(0, 90, 0); // Rotación de apertura
    private Quaternion posicionCerrada;
    private Quaternion posicionAbierta;
    private bool abierta = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posicionCerrada = transform.rotation;
        posicionAbierta = Quaternion.Euler(transform.eulerAngles + angulo);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Abrir()
    {
        if (!abierta)
        {
            transform.rotation = posicionAbierta;
            abierta = true;
        }
    }

    public void Cerrar()
    {
        if (abierta)
        {
            transform.rotation = posicionCerrada;
            abierta = false;
        }
    }
}
