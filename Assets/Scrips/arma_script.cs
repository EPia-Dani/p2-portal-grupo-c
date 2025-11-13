using UnityEngine;
using UnityEngine.InputSystem;

public class arma_script : MonoBehaviour
{
    public float distancia = 5f;     
    public float fuerza = 700f;       
    private GameObject item;        
    private Rigidbody rbItem;
    private Vector3 grab;
    [SerializeField] Transform player;


    void Update()
    {
        grab = new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z);
        // rayo para ver linea, luego quitar
        Debug.DrawRay(grab, -transform.forward * distancia, Color.red);

        if (item != null && item.CompareTag("Turret"))
        {
            item.transform.rotation = player.rotation;
            Vector3 destino = transform.position + -transform.forward * 1.5f;
        }

        // si hay cubo que se quede delante
        if (item != null)
        {
            item.transform.position = grab + -transform.forward * 3f;
        }

       
        if (Input.GetMouseButtonDown(0))
        {
            if (item == null)
            {
                //IntentarAgarrar();
            }
            else
            {
                //DispararObjeto();
            }
        }
        else if (Input.GetMouseButtonDown(1) && item != null)
        {
            //SoltarObjeto();
        }
    }

    void IntentarAgarrar()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(grab, -transform.forward, out hit, distancia))
        {
            //tag
            if (hit.collider.CompareTag("Cube") || hit.collider.CompareTag("Turret"))
            {
                item = hit.collider.gameObject;
                rbItem = item.GetComponent<Rigidbody>();
                rbItem.useGravity = false;
                if (hit.collider.CompareTag("Turret"))
                {
                    item.transform.rotation = player.rotation;
                }
               
            }
        }
    }
    void DispararObjeto()
    {
        rbItem.useGravity = true;
        rbItem.AddForce(-transform.forward * fuerza);
        item = null;
        rbItem = null;
    }

    void SoltarObjeto()
    {
        rbItem.useGravity = true;
        item = null;
        rbItem = null;
    }

    public void OnGrabObject(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (rbItem != null)
        {
            DispararObjeto();
            return;
        }

        IntentarAgarrar();
    }
    public void OnLeaveObject(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (rbItem != null)
            SoltarObjeto();
    }
}