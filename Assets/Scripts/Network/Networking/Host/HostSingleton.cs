using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;
    private HostGameManager m_GameManager;

    public static HostSingleton s_Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<HostSingleton>();

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

    public void CreateHost()
    {
        m_GameManager = new HostGameManager();
    }
}
