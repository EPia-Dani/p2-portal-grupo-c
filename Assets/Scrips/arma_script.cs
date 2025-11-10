using UnityEngine;

public class RayDebug : MonoBehaviour
{
    public float distancia = 5f;     
    public float fuerza = 700f;       
    private GameObject cub;        
    private Rigidbody rbCub;       


    void Update()
    {
        // rallo para ver linea, luego quitar
        //Debug.DrawRay(transform.position, -transform.forward * distancia, Color.red);

        // si hay cubo que se quede delante 1m
        if (cub != null)
        {
            cub.transform.position = transform.position + -transform.forward * 2f;
        }

       
        if (Input.GetMouseButtonDown(0))
        {
            if (cub == null)
            {
                IntentarAgarrar();
            }
            else
            {
                DispararObjeto();
            }
        }
        else if (Input.GetMouseButtonDown(1) && cub != null)
        {
            SoltarObjeto();
        }
    }

    void IntentarAgarrar()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, -transform.forward, out hit, distancia))
        {
            //tag
            if (hit.collider.CompareTag("Cube") || hit.collider.CompareTag("Turret"))
            {
                cub = hit.collider.gameObject;
                rbCub = cub.GetComponent<Rigidbody>();
                rbCub.useGravity = false;
                //rbCub.linearVelocity = Vector3.zero;
            }
        }
    }

    void DispararObjeto()
    {
        rbCub.useGravity = true;
        rbCub.AddForce(-transform.forward * fuerza);
        cub = null;
        rbCub = null;
    }

    void SoltarObjeto()
    {
        rbCub.useGravity = true;
        cub = null;
        rbCub = null;
    }
}