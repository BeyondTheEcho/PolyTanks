using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private LobbyItem m_LobbyItemPrefab;
    [SerializeField] private Transform m_LobbyItemParent;

    private bool m_IsJoining = false;
    private bool m_IsRefreshing = false;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (m_IsRefreshing) return;
        m_IsRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new();
            //How many lobbies to grab
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),

                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0")
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in m_LobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(m_LobbyItemPrefab, m_LobbyItemParent);

                lobbyItem.Initialize(this, lobby);
            }
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }

        m_IsRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (m_IsJoining) return;
        m_IsJoining = true;

        try
        {
            Lobby joinLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joinLobby.Data["JoinCode"].Value;

            await ClientSingleton.s_Instance.m_GameManager.StartClientAsync(joinCode);
        }
        catch(LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }

        m_IsJoining = false;
    }
}
