using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health m_Health;
    [SerializeField] private Image m_HealthBarImage;

    public void SetHealth(Health health)
    {
        m_Health = health;
    }

    public void Start()
    {
        m_Health.m_CurrentHealth.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, m_Health.m_CurrentHealth.Value);
    }

    public void OnDestroy()
    {
        m_Health.m_CurrentHealth.OnValueChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(int oldHealth, int newHealth)
    {
        m_HealthBarImage.fillAmount = (float) newHealth / m_Health.m_MaxHealth;
    }
}