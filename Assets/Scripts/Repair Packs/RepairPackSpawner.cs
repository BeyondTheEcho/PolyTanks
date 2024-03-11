using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RepairPackSpawner : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject m_RepairPackPrefab;
    [SerializeField] private Transform[] m_SpawnPoints;

    [Header("Settings")]
    [SerializeField] private int m_MaxRepairPacks = 3;
    [SerializeField] private int m_RepairPackTotalRepairValue = 100;

    //vars
    private float m_RepairRadius;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        m_RepairRadius = m_RepairPackPrefab.GetComponent<SphereCollider>().radius;

        SpawnRepairPacks();
    }

    private void SpawnRepairPacks()
    {
        // Get a list of unoccupied spawn points
        List<Transform> unoccupiedSpawnPoints = new List<Transform>();
        foreach (Transform spawnPoint in m_SpawnPoints)
        {
            Collider[] colliders = Physics.OverlapSphere(spawnPoint.position, m_RepairRadius);
            bool isOccupied = false;
            foreach (Collider collider in colliders)
            {
                if (collider.GetComponent<RespawningRepairPack>() != null || collider.GetComponent<Health>() != null)
                {
                    isOccupied = true;
                    break;
                }
            }
            if (!isOccupied)
            {
                unoccupiedSpawnPoints.Add(spawnPoint);
            }
        }

        // Spawn repair packs
        int packsToSpawn = Mathf.Min(m_MaxRepairPacks, unoccupiedSpawnPoints.Count);
        for (int i = 0; i < packsToSpawn; i++)
        {
            Transform spawnPoint = unoccupiedSpawnPoints[Random.Range(0, unoccupiedSpawnPoints.Count)];
            var repairPack = Instantiate(m_RepairPackPrefab, spawnPoint.position, Quaternion.identity);
            repairPack.GetComponent<RespawningRepairPack>().SetValue(m_RepairPackTotalRepairValue);

            repairPack.gameObject.GetComponent<NetworkObject>().Spawn();
        }
    }


}
