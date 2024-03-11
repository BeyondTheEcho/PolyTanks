using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class RepairPack : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer m_MeshRenderer;

    [Header("Settings")]
    [SerializeField] protected int m_TotalRepairValue;
    [SerializeField] protected int m_RepairValuePerInterval;
    [SerializeField] protected float m_RepairInterval;

    //Vars
    protected bool m_AlreadyCollected;
    protected int m_RemainingRepairValue;

    public void SetValue(int value)
    {
        m_TotalRepairValue = value;
    }
    
    protected void Show(bool show)
    {
        m_MeshRenderer.enabled = show;
    }
}
