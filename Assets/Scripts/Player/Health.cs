using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private GameObject m_UIPrefab;
    [SerializeField] private ParticleSystem m_DamageEffectOne;
    [SerializeField] private ParticleSystem m_DamageEffectTwo;
    [SerializeField] private ParticleSystem m_DamageEffectThree;
    [field: SerializeField] public int m_MaxHealth { get ; private set; } = 100;
    public NetworkVariable<int> m_CurrentHealth = new NetworkVariable<int>();

    private GameObject m_UIInstance;
    private bool m_IsDead = false;
    public bool m_IsRepairing = false;

    public Action<Health> a_OnDie;

    private void Start()
    {
        // Instantiate the UI prefab only on the client that owns this object
        if (IsOwner)
        {
            m_UIInstance = Instantiate(m_UIPrefab);

            m_UIInstance.GetComponentInChildren<HealthDisplay>().SetHealth(this);
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

        if (IsServer)
        {
            // Trigger damage effects on all clients
            TargetTriggerDamageEffectsClientRpc();
        }

        if (m_CurrentHealth.Value == 0)
        {
            a_OnDie?.Invoke(this);
            m_IsDead = true;
        }
    }

    [ClientRpc]
    private void TargetTriggerDamageEffectsClientRpc()
    {
        if (m_CurrentHealth.Value <= 0)
        {
            m_DamageEffectOne.Stop();
            m_DamageEffectTwo.Stop();
            m_DamageEffectThree.Stop();
            return;
        }

        if (m_CurrentHealth.Value <= (m_MaxHealth * 0.25f))
        {
            m_DamageEffectThree.Play();
        }
        else
        {
            m_DamageEffectThree.Stop();
        }

        if (m_CurrentHealth.Value <= (m_MaxHealth * 0.5f))
        {
            m_DamageEffectTwo.Play();
        }
        else
        {
            m_DamageEffectTwo.Stop();
        }

        if (m_CurrentHealth.Value <= (m_MaxHealth * 0.75f))
        {
            m_DamageEffectOne.Play();
        }
        else
        {
            m_DamageEffectOne.Stop();
        }
    }
}
