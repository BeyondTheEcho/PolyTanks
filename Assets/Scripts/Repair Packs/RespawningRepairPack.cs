using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawningRepairPack : RepairPack
{
    private List<(Coroutine coroutine, GameObject player)> m_ActiveRepairList;

    private void Awake()
    {
        m_ActiveRepairList = new();
        m_RemainingRepairValue = m_TotalRepairValue;
    }

    public override int Collect()
    {
        if (!IsServer) 
        {
            Show(false);
            return 0;
        }

        if (m_AlreadyCollected) return 0;

        m_AlreadyCollected = true;
        return m_TotalRepairValue;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.attachedRigidbody.gameObject.TryGetComponent(out Health health))
        {
            if (health.m_IsRepairing) return;

            var routine = StartCoroutine(StartRepair(health));
            health.m_IsRepairing = true;

            m_ActiveRepairList.Add((routine, col.attachedRigidbody.gameObject));
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.attachedRigidbody.gameObject.TryGetComponent(out Health health))
        {
            health.m_IsRepairing = false;

            var repairsToRemove = m_ActiveRepairList.FindAll(repair => repair.player == col.attachedRigidbody.gameObject && repair.coroutine != null);
            foreach (var repair in repairsToRemove)
            {
                StopCoroutine(repair.coroutine);
                m_ActiveRepairList.Remove(repair);
            }
        }
    }

    public IEnumerator StartRepair(Health health)
    {
        if (!IsServer) 
        {
            health.m_IsRepairing = false;
            yield break;
        }

        while (m_RemainingRepairValue >= m_RepairValuePerInterval)
        {
            health.RestoreHealth(m_RepairValuePerInterval);
            m_RemainingRepairValue -= m_RepairValuePerInterval;
            yield return new WaitForSeconds(m_RepairInterval);
        }

        Show(false);
        health.m_IsRepairing = false;
    }
}
