using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_NameField;
    [SerializeField] private Button m_ConnectionButton;
    [SerializeField] private int m_MinNameLength = 1;
    [SerializeField] private int m_MaxNameLength = 20;

    public const string c_PlayerNameKey = "PlayerName";

    private void Start()
    {
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        m_NameField.text = PlayerPrefs.GetString(c_PlayerNameKey, string.Empty);
        HandleNameChange();
    }

    public void HandleNameChange()
    {
        m_ConnectionButton.interactable = (
            m_NameField.text.Length >= m_MinNameLength && 
            m_NameField.text.Length <= m_MaxNameLength);
    }

    public void Connect()
    {
        PlayerPrefs.SetString(c_PlayerNameKey, m_NameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
