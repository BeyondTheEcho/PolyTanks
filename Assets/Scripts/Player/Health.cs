using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private GameObject m_UIPrefab;
    [field: SerializeField] public int m_MaxHealth { get ; private set; } = 100;
    public NetworkVariable<int> m_CurrentHealth = new NetworkVariable<int>();
    public bool m_IsRepairing = false;

    private GameObject m_UIInstance;
    private bool m_IsDead = false;

    public Action<Health> a_OnDie;

    private void Start()
    {
        // Instantiate the UI prefab only on the client that owns this object
        if (IsOwner)
        {
            m_UIInstance = Instantiate(m_UIPrefab);
            DontDestroyOnLoad(m_UIInstance);

            if(m_UIInstance.TryGetComponent<HealthDisplay>(out HealthDisplay healthDisplay))
            {
                Debug.Log("Found Display");
                healthDisplay.SetHealth(this);
                return;
            }
        }
    }

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
