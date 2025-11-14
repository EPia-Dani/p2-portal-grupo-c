using System;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer laser; 
    public Transform tPoint; 
    public GameObject player; 
    public GameObject turret;
    public GameObject cube; 
    public float laserRange = 20f; 
    public bool isActive = true;
    public static event Action playerDie;
    void Start()
    {  
        laser = gameObject.AddComponent<LineRenderer>();
        laser.startWidth = 0.05f; 
        laser.endWidth = 0.05f; 
        laser.material = new Material(Shader.Find("Unlit/Color")); 
        laser.material.color = Color.red; 
    }
  
     void Update()
    {
        if (isActive)
        {
            ShootLaser();
        }
    }

    private void ShootLaser()
    {
        laser.enabled = true;
        laser.SetPosition(0, tPoint.position);
        RaycastHit hit;
        if (Physics.Raycast(tPoint.position, tPoint.forward, out hit, laserRange))
        {
            laser.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Player"))
            {

                playerDie?.Invoke();
            }


            Laser otherTurret = hit.collider.GetComponent<Laser>();
            if (otherTurret == null)
            {
                otherTurret = hit.collider.GetComponentInParent<Laser>();
            }
            if (otherTurret != null && otherTurret != this)
            {
                otherTurret.laser.enabled = false;
                otherTurret.isActive = false;
            }
        }
        else
        {
            laser.SetPosition(1, tPoint.position + tPoint.forward * laserRange);
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cube") || collision.gameObject.CompareTag("Turret"))
        {
            isActive = false;
            laser.enabled = false;
        }  
    }
}
