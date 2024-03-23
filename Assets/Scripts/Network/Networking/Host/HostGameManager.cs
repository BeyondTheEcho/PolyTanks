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
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Text;
using Unity.Services.Authentication;

public class HostGameManager : IDisposable
{
    private Allocation m_Allocation;
    private NetworkServer m_NetworkServer;
    private String m_JoinCode;
    private String m_LobbyID;
    private const string c_GameSceneName = "Game";
    private const int c_MaxConnections = 20;
    private const int c_HeartBeatTime = 15;

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

        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>()
            {
                {"JoinCode", new DataObject(visibility: DataObject.VisibilityOptions.Member, value: m_JoinCode)}
            };

            string playerName = PlayerPrefs.GetString(NameSelector.c_PlayerNameKey, "Missing Name");

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", c_MaxConnections, lobbyOptions);

            m_LobbyID = lobby.Id;

            HostSingleton.s_Instance.StartCoroutine(HeartBeatLobby(c_HeartBeatTime));
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }

        m_NetworkServer = new NetworkServer(NetworkManager.Singleton);

        UserData userData = new UserData 
        {
            m_UserName = PlayerPrefs.GetString(NameSelector.c_PlayerNameKey, "Missing Name"),
            m_UserAuthID = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(c_GameSceneName, LoadSceneMode.Single);
    }

    private IEnumerator HeartBeatLobby(float waitTimeSeconds)
    {
        WaitForSecondsRealtime realtimeDelay = new(waitTimeSeconds);

        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(m_LobbyID);
            yield return realtimeDelay;
        }
    }

    public async void Dispose()
    {
        HostSingleton.s_Instance.StopCoroutine(nameof(HeartBeatLobby));

        if (!string.IsNullOrEmpty(m_LobbyID))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(m_LobbyID);
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogError(ex);
            }

            m_LobbyID = string.Empty;
        }

        m_NetworkServer?.Dispose();
    }
}