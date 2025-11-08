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

            // laser toca jugador muere 
            if (hit.collider.gameObject == player)
            {
                
                Destroy(player); 
            }

            // Si el láser toca otra torreta, la destruye
            turrets otherTurret = hit.collider.GetComponent<turrets>();

            // Si no lo encuentra en el objeto directo, busca en el padre
            if (otherTurret == null)
            {
                Debug.Log("null torret");

                otherTurret = hit.collider.GetComponentInParent<turrets>();
            }

            // Si encontró otra torreta y no es esta misma
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
        if (collision.gameObject == cube || collision.gameObject == turret)
        {
            isActive = false; 
        }
    }

    public void CogerTorret(Transform gravityGun)
    {
        // coger torret
        torretSost = true;
        //desactyivar laser mientras la tenga sugetada
        isActive = false; 
        transform.SetParent(gravityGun);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // laser hacia adelante
        puntoT.forward = gravityGun.forward;
    }

    public void SoltarTorret()
    {
        // Suelta la torreta y reactiva laser
        torretSost = false;
        isActive = true; 
        transform.SetParent(null);
    }
}
