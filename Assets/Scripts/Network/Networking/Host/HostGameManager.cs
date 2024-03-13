using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class HostGameManager
{
    private Allocation m_Allocation;
    private String m_JoinCode;
    private const string c_GameSceneName = "Game";
    private const int c_MaxConnections = 20;

    public async Task StartHostAsync()
    {
        try
        {
            m_Allocation = await Relay.Instance.CreateAllocationAsync(c_MaxConnections);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return;
        }

        try
        {
            m_JoinCode = await Relay.Instance.GetJoinCodeAsync(m_Allocation.AllocationId);
            Debug.Log($"Join Code: {m_JoinCode}");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        //switch dtls to udp if people have trouble connecting
        RelayServerData relayServerData = new(m_Allocation, "dtls");

        transport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(c_GameSceneName, LoadSceneMode.Single);
    }
}
