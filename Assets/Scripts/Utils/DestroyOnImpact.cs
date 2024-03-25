using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_CollisionEffectPrefab;
    [SerializeField] private CinemachineImpulseSource m_ExplosionImpulse;

    private void OnTriggerEnter(Collider col)
    {
        if (m_CollisionEffectPrefab != null)
        {
            // Get the point of contact
            Vector3 contactPoint = col.ClosestPointOnBounds(transform.position);

            // Instantiate the effect at the contact point
            var explosion = Instantiate(m_CollisionEffectPrefab, contactPoint, Quaternion.identity);
            explosion.Play();

            //Transmit Impulse
            m_ExplosionImpulse.GenerateImpulse();

            // Destroy the explosion object after fixed time
            Destroy(explosion.gameObject, 3f);
        }

        Destroy(gameObject);
    }
}
