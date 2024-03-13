using System.Collections;
using UnityEngine;

public class PlayParticleSystemRandom : MonoBehaviour
{
    [SerializeField] private GameObject[] m_GameObjects;
    [SerializeField] private float m_MinInterval = 5f;
    [SerializeField] private float m_MaxInterval = 20f;
    [SerializeField] private float m_MinPitch = 0.8f;
    [SerializeField] private float m_MaxPitch = 1.2f;

    private void Start()
    {
        StartCoroutine(PlayRandomly());
    }

    private IEnumerator PlayRandomly()
    {
        while (true)
        {
            float interval = Random.Range(m_MinInterval, m_MaxInterval);
            yield return new WaitForSeconds(interval);

            if (m_GameObjects != null && m_GameObjects.Length > 0)
            {
                int randomIndex = Random.Range(0, m_GameObjects.Length);
                GameObject randomGameObject = m_GameObjects[randomIndex];
                if (randomGameObject != null)
                {
                    ParticleSystem particleSystem = randomGameObject.GetComponent<ParticleSystem>();
                    AudioSource audioSource = randomGameObject.GetComponent<AudioSource>();
                    if (particleSystem != null)
                    {
                        particleSystem.Play();
                    }
                    if (audioSource != null)
                    {
                        // Adjust the pitch between the specified ranges
                        audioSource.pitch = Random.Range(m_MinPitch, m_MaxPitch);
                        audioSource.Play();
                    }
                }
            }
        }
    }
}
