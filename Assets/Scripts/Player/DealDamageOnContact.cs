using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int m_Damage = 5;
    private ulong m_OwnerClientID;

    public void SetOwner(ulong ownerClientID)
    {
        m_OwnerClientID = ownerClientID;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.attachedRigidbody == null) return;

        if (collider.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (netObj.OwnerClientId == m_OwnerClientID) return;
        }
        
        if (collider.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(m_Damage);
        }
    }
}
