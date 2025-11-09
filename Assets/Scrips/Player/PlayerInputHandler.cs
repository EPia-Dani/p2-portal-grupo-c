using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static event Action<bool> AimingChanged;
    public static event Action<bool> ShootingHeldChanged;

    public static event Action<Vector2> MoveChanged;
    public static event Action<Vector2> LookDelta;
    public static event Action<bool> SprintChanged;
    public static event Action JumpRequest;
    public static event Action RestarGame;
    private bool playerDead = false;

    private void OnEnable()
    {
        //RedButton.RedButtonArea += UpdateRedButtonArea;
        //PlayerStats.OnPlayerDeath += () => playerDead = true;
    }
    private void OnDisable()
    {
        //RedButton.RedButtonArea -= UpdateRedButtonArea;
        //PlayerStats.OnPlayerDeath -= () => playerDead = true;

    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        AimingChanged = null;
        ShootingHeldChanged = null;
      

        MoveChanged = null;
        LookDelta = null;
        SprintChanged = null;
        JumpRequest = null;

    }

    public void OnAim(InputAction.CallbackContext c)
    {
        if (c.started) { AimingChanged?.Invoke(true); }
        if (c.canceled) { AimingChanged?.Invoke(false); }
    }

    public void OnShoot(InputAction.CallbackContext c)
    {
        Debug.Log("Shooting Input Detected");
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
