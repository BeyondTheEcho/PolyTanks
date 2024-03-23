using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    public HostGameManager m_GameManager {get; private set;}

    public static HostSingleton s_Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<HostSingleton>();

            if(instance == null)
            {
                Debug.LogError("No HostSingleton Found");
                return null;
            }

            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void CreateHost()
    {
        m_GameManager = new HostGameManager();
    }

    private void OnDestroy()
    {
        m_GameManager.Dispose();
    }
}
