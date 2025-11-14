using UnityEngine;

public class DispenserCube : MonoBehaviour
{
    [SerializeField] public GameObject cubPrefab;
    [SerializeField] public Transform dispenserPoint;
    private int i = 1;

    public void DropCube()
    {
        if(i != 1)
        {
            return;
        }
        Vector3 posicion = dispenserPoint.position + Vector3.up * -1;
        Instantiate(cubPrefab, posicion, Quaternion.identity);
        i++;

    }
    public void ResetI ()
    {
        i = 1;
    }
}
