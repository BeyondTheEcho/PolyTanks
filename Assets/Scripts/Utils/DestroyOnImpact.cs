using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_CollisionEffectPrefab;

    private void OnTriggerEnter(Collider col)
    {
        if (m_CollisionEffectPrefab != null)
        {
            // Get the point of contact
            Vector3 contactPoint = col.ClosestPointOnBounds(transform.position);
            Quaternion contactRotation = Quaternion.LookRotation(contactPoint - transform.position);

            // Instantiate the effect at the contact point with the contact rotation
            var explosion = Instantiate(m_CollisionEffectPrefab, contactPoint, contactRotation);
            explosion.Play();

            // Destroy the explosion object after fixed time
            Destroy(explosion.gameObject, 3f);
        }

        Destroy(gameObject);
    }
}
