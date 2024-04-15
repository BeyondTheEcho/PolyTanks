using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static event Action<Player> a_OnPlayerSpawned;
    public static event Action<Player> a_OnPlayerDespawned;

    [field: SerializeField] public Health m_Health {get; private set;}

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            a_OnPlayerSpawned?.Invoke(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            a_OnPlayerDespawned?.Invoke(this);
        }
    }
}
