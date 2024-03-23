using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
    public ClientGameManager m_GameManager {get; private set;}

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

    public async Task<bool> CreateClientAsync()
    {
        m_GameManager = new ClientGameManager();

        return await m_GameManager.InitAsync();
    }

    private void OnDestroy()
    {
        m_GameManager?.Dispose();
    }
}
