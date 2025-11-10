using UnityEngine;

public class turrets : MonoBehaviour
{
    public LineRenderer laser; 
    public Transform puntoT; 
    public GameObject player; 
    public GameObject turret;
    public GameObject cube; 
    public float laserRange = 20f; 
    public bool isActive = true;
    public bool torretSost = false; 
      void Start()
    {

        
        //laser
        laser = gameObject.AddComponent<LineRenderer>();
        laser.startWidth = 0.05f; 
        laser.endWidth = 0.05f; 
        laser.material = new Material(Shader.Find("Unlit/Color")); 
        laser.material.color = Color.red; 
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

            // El láser toca al jugador, lo elimina cambiar a gzame object
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("player torret");
                Destroy(hit.collider.gameObject);
            }
            

            // Si el láser toca otra torreta la destryte 
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
        if (collision.gameObject.CompareTag("Cube") || collision.gameObject.CompareTag("Turret"))
        {
            Debug.Log("Torreta desactivada por colisión");
            isActive = false; 
            laser.enabled = false;

        }
       
    }

   
}
