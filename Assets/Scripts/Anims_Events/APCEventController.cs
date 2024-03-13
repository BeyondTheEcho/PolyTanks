using System.Collections;
using UnityEngine;

public class APCEventController : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_MuzzleFlash;
    [SerializeField] private ParticleSystem m_CasingEjection;
    [SerializeField] private AudioSource m_FireSound;

    public void TriggerFireEffects()
    {
        if (m_MuzzleFlash != null)
        {
            m_MuzzleFlash.Play();
        }

        if (m_CasingEjection != null)
        {
            m_CasingEjection.Play();
        }

        if (m_FireSound != null)
        {
            m_FireSound.Play();
        }
    }

    public void EndFireEffects()
    {
        if (m_MuzzleFlash != null)
        {
            m_MuzzleFlash.Stop();
        }

        if (m_CasingEjection != null)
        {
            m_CasingEjection.Stop();
        }

        if (m_FireSound != null)
        {
            m_FireSound.Stop();
        }
    }
}
