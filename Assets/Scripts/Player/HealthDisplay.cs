using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health m_Health;
    [SerializeField] private Image m_HealthBarImage;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        m_Health.m_CurrentHealth.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, m_Health.m_CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;

        m_Health.m_CurrentHealth.OnValueChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(int oldHealth, int newHealth)
    {
        m_HealthBarImage.fillAmount = (float) newHealth / m_Health.m_MaxHealth;
    }
}