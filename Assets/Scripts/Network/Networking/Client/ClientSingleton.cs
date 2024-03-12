using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
    private ClientGameManager m_GameManager;

    public static ClientSingleton s_Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<ClientSingleton>();

            if(instance == null)
            {
                Debug.LogError("No ClientSingleton Found");
                return null;
            }

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public async Task CreateClientAsync()
    {
        m_GameManager = new ClientGameManager();

        try
        {
            await m_GameManager.InitAsync();
            Debug.Log("Client initialization completed successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Client initialization failed: {ex.Message}");
        }
    }
}
