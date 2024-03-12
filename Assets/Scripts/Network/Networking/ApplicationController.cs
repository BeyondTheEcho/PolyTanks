using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton m_ClientPrefab;
    [SerializeField] private HostSingleton m_HostPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInModeAsync(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInModeAsync(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            var clientSingleton = Instantiate(m_ClientPrefab);
            DontDestroyOnLoad(clientSingleton);
            bool authenticated = await clientSingleton.CreateClientAsync();

            var hostSingleton = Instantiate(m_HostPrefab);
            DontDestroyOnLoad(hostSingleton);
            hostSingleton.CreateHost();

            if (authenticated)
            {
                clientSingleton.m_GameManager.GoToMenu();
            }
        }
    }
}
