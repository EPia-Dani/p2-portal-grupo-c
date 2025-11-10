using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static event Action<bool> ShootingBluePortal;
    public static event Action<bool> ShootingOrangePortal;

    public static event Action<Vector2> MoveChanged;
    public static event Action<Vector2> LookDelta;
    public static event Action<bool> SprintChanged;
    public static event Action JumpRequest;
    public static event Action RestarGame;
    private bool playerDead = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        ShootingBluePortal = null;
        ShootingOrangePortal = null;
      

        MoveChanged = null;
        LookDelta = null;
        SprintChanged = null;
        JumpRequest = null;

    }

    public void OnShootBlue(InputAction.CallbackContext c)
    {
        Debug.Log("Shooting Blue Portal");
    }

    public void OnShootOrange(InputAction.CallbackContext c)
    {
        Debug.Log("Shooting Orange Portal");
    }

    public void OnMove(InputAction.CallbackContext c)
    {
        if (c.performed || c.canceled)
            MoveChanged?.Invoke(c.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext c)
    {
        if (c.performed || c.canceled)
            LookDelta?.Invoke(c.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext c)
    {
        if (c.performed) SprintChanged?.Invoke(true);
        if (c.canceled) SprintChanged?.Invoke(false);
    }

    public void OnJump(InputAction.CallbackContext c)
    {
        if (c.performed) JumpRequest?.Invoke();
    }
}
