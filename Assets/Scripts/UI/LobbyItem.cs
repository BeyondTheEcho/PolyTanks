using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text m_LobbyNameText;
    [SerializeField] private TMP_Text m_LobbyPlayersText;

    private LobbiesList m_LobbiesList;
    private Lobby m_Lobby;

    public void Initialize(LobbiesList lobbiesList, Lobby lobby)
    {
        m_LobbiesList = lobbiesList;
        m_Lobby = lobby;

        m_LobbyNameText.text = lobby.Name;
        m_LobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        m_LobbiesList.JoinAsync(m_Lobby);
    }
}
