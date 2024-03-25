using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Health : NetworkBehaviour
{

    [field: SerializeField] public int m_MaxHealth { get ; private set; } = 100;
    public NetworkVariable<int> m_CurrentHealth = new NetworkVariable<int>();
    public bool m_IsRepairing = false;

    private bool m_IsDead = false;

    public Action<Health> a_OnDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        m_CurrentHealth.Value = m_MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        ModifyHealth(-damage);
    }

    public void RestoreHealth(int heal)
    {
        ModifyHealth(heal);
    }

    private void ModifyHealth(int value)
    {
        if (m_IsDead) return;

        int newHealth = m_CurrentHealth.Value + value;
        m_CurrentHealth.Value = Mathf.Clamp(newHealth, 0, m_MaxHealth);

        if (m_CurrentHealth.Value == 0)
        {
            a_OnDie?.Invoke(this);
            m_IsDead = true;
        }
    }
}
