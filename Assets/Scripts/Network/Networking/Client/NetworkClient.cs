using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager m_NetworkManager;
    private const string c_MenuSceneName = "MainMenu";

    public NetworkClient(NetworkManager networkManager)
    {
        m_NetworkManager = networkManager;

        m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientID)
    {
        //Ensures the server does not execute
        if (clientID != 0 && clientID != m_NetworkManager.LocalClientId) return;

        if (SceneManager.GetActiveScene().name != c_MenuSceneName)
        {
            SceneManager.LoadScene(c_MenuSceneName);
        }

        if (m_NetworkManager.IsConnectedClient)
        {
            m_NetworkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if (m_NetworkManager == null) return;
        
        m_NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        
    }
}
