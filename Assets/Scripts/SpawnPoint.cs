using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> m_SpawnPoints = new();

    private void OnEnable()
    {
        m_SpawnPoints.Add(this);
    }

    private void OnDisable()
    {
        m_SpawnPoints.Remove(this);
    }

    public static Vector3 GetRandomSpawnPoint()
    {
        if (m_SpawnPoints.Count == 0)
        {
            return Vector3.zero;
        }

        return m_SpawnPoints[Random.Range(0, m_SpawnPoints.Count)].transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
