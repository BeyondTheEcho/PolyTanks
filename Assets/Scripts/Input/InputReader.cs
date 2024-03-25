using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action<bool> a_PrimaryFire;
    public event Action<bool> a_Aim;
    public event Action a_EscapePressed;
    public event Action<Vector2> a_PlayerMovement;
    
    public Vector2 m_MousePosition {get; private set;}
    public Vector2 m_MouseDelta {get; private set;}
    private Controls m_Controls;

    private void OnEnable()
    {
        if (m_Controls == null)
        {
            m_Controls = new Controls();
            m_Controls.Player.SetCallbacks(this);
        }

        //How to enable / disable different Action Maps
        m_Controls.Player.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        a_PlayerMovement?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed) a_PrimaryFire?.Invoke(true);
        if (context.canceled) a_PrimaryFire?.Invoke(false);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed) a_Aim?.Invoke(true);
        if (context.canceled) a_Aim?.Invoke(false);
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.performed) a_EscapePressed?.Invoke();
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        m_MousePosition = context.ReadValue<Vector2>();
    }

    public void OnMouseDelta(InputAction.CallbackContext context)
    {
        m_MouseDelta = context.ReadValue<Vector2>();
    }
}
