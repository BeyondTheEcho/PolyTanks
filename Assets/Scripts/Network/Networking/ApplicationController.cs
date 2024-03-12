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

            await clientSingleton.CreateClientAsync();

            //go to main menu
        }
    }
}
