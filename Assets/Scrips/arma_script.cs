using UnityEngine;

public class arma_script : MonoBehaviour
{

    [SerializeField] public float distanciaMaxima = 5f;
    public Transform puntoFlotante;
    [SerializeField] public float velocidadFlotante = 10f;
    private GameObject cuboAgarrado;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (cuboAgarrado == null)
            {
                //camara por aliniacion
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, distanciaMaxima))
                {
                    if (hit.collider.CompareTag("Cube"))
                    {
                        cuboAgarrado = hit.collider.gameObject;
                        cuboAgarrado.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
            }
            else
            {
                // lanza el cubo 
                //iskinematic igual a flase para que responda a las fisicas gravedad y eso
                cuboAgarrado.GetComponent<Rigidbody>().isKinematic = false;
                cuboAgarrado.transform.position = puntoFlotante.position;
                cuboAgarrado.GetComponent<Rigidbody>().AddForce(transform.forward * 500);
                cuboAgarrado = null;
            }
        }
        else if (Input.GetMouseButtonDown(1) && cuboAgarrado != null)
        {
            // Soltar el cubo 
            cuboAgarrado.GetComponent<Rigidbody>().isKinematic = false;
            cuboAgarrado = null;
        }

        // flotando delante 
        if (cuboAgarrado != null)
        {
            cuboAgarrado.transform.position = Vector3.Lerp(
                cuboAgarrado.transform.position,
                puntoFlotante.position,
                Time.deltaTime * velocidadFlotante
            );
        }

    }
}
