using UnityEngine;
using UnityEngine.UI;

public class Crossair : MonoBehaviour
{
    [SerializeField] private Sprite AllPortals;
    [SerializeField] private Sprite BluePortal;
    [SerializeField] private Sprite OrangePortal;
    [SerializeField] private Sprite NoPortals;
    [SerializeField] private Image crossairImage;

    private void OnEnable()
    {
        ShootingPortals.ShootingBluePortal += ShootingBluePortal;
        ShootingPortals.ShootingOrangePortal += ShootingOrangePortal;
    }
    private void ShootingOrangePortal()
    {
        if (crossairImage.sprite.Equals(AllPortals))
        {
            crossairImage.sprite = BluePortal;
        }
        else
        {
            crossairImage.sprite = NoPortals;

        }
    }
    private void ShootingBluePortal()
    {
        if (crossairImage.sprite.Equals(AllPortals))
        {
            crossairImage.sprite = OrangePortal;
        }
        else
        {
            crossairImage.sprite = NoPortals;

        }
    }
}
