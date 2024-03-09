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
    public event Action<Vector2> a_PlayerMovement;
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
}
