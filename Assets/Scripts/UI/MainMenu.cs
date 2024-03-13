using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_JoinCodeField;

    public async void StartHost()
    {
        await HostSingleton.s_Instance.m_GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.s_Instance.m_GameManager.StartClientAsync(m_JoinCodeField.text);
    }
}
