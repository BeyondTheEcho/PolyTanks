using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject m_PlayerPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        Player[] players = FindObjectsOfType<Player>();

        foreach (Player player in players)
        {
            RespawnExistingPlayers(player);
            //HandlePlayerSpawn(player);
        }

        Player.a_OnPlayerSpawned += HandlePlayerSpawn;
        Player.a_OnPlayerDespawned += HandlePlayerDespawn;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        Player.a_OnPlayerSpawned -= HandlePlayerSpawn;
        Player.a_OnPlayerDespawned -= HandlePlayerDespawn;
    }

    private void HandlePlayerSpawn(Player player)
    {
        player.m_Health.a_OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawn(Player player)
    {
        player.m_Health.a_OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(Player player)
    {
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId)
    {
        yield return null;

        NetworkObject player = Instantiate(m_PlayerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);

        player.SpawnAsPlayerObject(ownerClientId);
    }

    private void RespawnExistingPlayers(Player player)
    {
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }
}
