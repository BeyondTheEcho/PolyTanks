using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image m_HealthBarImage;
    [SerializeField] private Crosshair m_Crosshair;

    private Health m_Health;

    public void SetHealth(Health health)
    {
        m_Health = health;
    }

    public void Start()
    {
        m_Health.m_CurrentHealth.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, m_Health.m_CurrentHealth.Value);
    }

    private void HandleHealthChange(int oldHealth, int newHealth)
    {
        m_HealthBarImage.fillAmount = (float) newHealth / m_Health.m_MaxHealth;
    }

    public void OnDestroy()
    {
        m_Health.m_CurrentHealth.OnValueChanged -= HandleHealthChange;
    }

    public Crosshair GetCrosshair()
    {
        return m_Crosshair;
    }
}