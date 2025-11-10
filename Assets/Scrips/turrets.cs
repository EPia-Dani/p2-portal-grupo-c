using UnityEngine;

public class turrets : MonoBehaviour
{
    public LineRenderer laser; // laser
    public Transform puntoT; //punto torre disparo
    public GameObject player; 
    public GameObject turret;
    public GameObject cube; 
    public float laserRange = 20f; // Alcance del láser
    public bool isActive = true; //ESTADO TORRE6A
    public bool torretSost = false; 
      void Start()
    {

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
        //laser
        laser = gameObject.AddComponent<LineRenderer>();
        laser.startWidth = 0.05f; // Ancho inicial del láser
        laser.endWidth = 0.05f; // Ancho final del láser
        laser.material = new Material(Shader.Find("Unlit/Color")); // Material 
        laser.material.color = Color.red; // Color 
    }
  
     void Update()
    {

        if (isActive && !torretSost)
        {
            ShootLaser();
        }
        else
        {
            laser.enabled = false;
        }
    }

    private void ShootLaser()
    {
        laser.enabled = true;
        laser.SetPosition(0, puntoT.position);
        RaycastHit hit;
        if (Physics.Raycast(puntoT.position, puntoT.forward, out hit, laserRange))
        {
            laser.SetPosition(1, hit.point);

            // El láser toca al jugador, lo elimina
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("player torret");
                Destroy(hit.collider.gameObject);
            }
            

            // Si el láser toca otra torreta, la destruye
            turrets otherTurret = hit.collider.GetComponent<turrets>();
            if (otherTurret == null)
            {
                otherTurret = hit.collider.GetComponentInParent<turrets>();
            }
            if (otherTurret != null && otherTurret != this)
            {
                Debug.Log("Torreta destruida por láser");
                Destroy(otherTurret.gameObject);
            }
        }
        else
        {
            laser.SetPosition(1, puntoT.position + puntoT.forward * laserRange);
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        // torreta golpeada por  cubo o  torreta desactivamos toore
        if (collision.gameObject.CompareTag("Cube"))
        {
            Debug.Log("Torreta desactivada por colisión con cubo");
            isActive = false; 
            laser.enabled = false;

        }
       
    }

   
}
