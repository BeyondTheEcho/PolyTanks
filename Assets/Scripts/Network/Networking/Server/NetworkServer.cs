using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager m_NetworkManager;
    private Dictionary<ulong, string> m_PlayerIDToAuth = new();
    private Dictionary<string, UserData> m_PlayerIDToUserData = new();

    public NetworkServer(NetworkManager networkManager)
    {
        m_NetworkManager = networkManager;

        m_NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        m_NetworkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        m_PlayerIDToAuth.Add(request.ClientNetworkId, userData.m_UserName);
        m_PlayerIDToUserData.Add(userData.m_UserName, userData);

        Debug.Log($"{userData.m_UserName} has joined");

        response.Approved = true;
        response.Position = SpawnPoint.GetRandomSpawnPoint();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientID)
    {
        if (m_PlayerIDToAuth.TryGetValue(clientID, out string authID))
        {
            m_PlayerIDToAuth.Remove(clientID);
            m_PlayerIDToUserData.Remove(authID);
        }
    }

    public void Dispose()
    {
        if (m_NetworkManager == null) return;

        m_NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
        m_NetworkManager.OnServerStarted -= OnNetworkReady;
        m_NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        if (m_NetworkManager.IsListening)
        {
            m_NetworkManager.Shutdown();
        }
    }
}
