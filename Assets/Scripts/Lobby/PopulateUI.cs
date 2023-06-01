using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Serialization;

public class PopulateUI : MonoBehaviour
{
    public TextMeshProUGUI lobbyName;
    public TextMeshProUGUI lobbyCode;
    public TextMeshProUGUI gameMode;

    public TMP_InputField newName;
    public TMP_InputField newPlayerLevel;

    public GameObject playerInfoContainer;
    public GameObject playerInfoPrefab;

    private CurrentLobby _currentLobby;

    private string lobbyId;

    private void Start()
    {
        _currentLobby = GameObject.Find("LobbyManager").GetComponent<CurrentLobby>();
        PopulateUIElements();
        lobbyId = _currentLobby.currentLobby.Id;
        InvokeRepeating(nameof(PollForLobbyUpdate), 1.1f,2f); // monobehaviour un kendi fonksiyonu
        LobbyStatic.LogPlayerInLobby(_currentLobby.currentLobby);
    }

    void PopulateUIElements()
    {
        lobbyName.text = _currentLobby.currentLobby.Name;
        lobbyCode.text = _currentLobby.currentLobby.LobbyCode;
        gameMode.text = _currentLobby.currentLobby.Data["GameMode"].Value;
        ClearContainer();
        foreach (Player player in _currentLobby.currentLobby.Players)
        {
            CreatePlayerInfoCard(player);
        }
    }

    void CreatePlayerInfoCard(Player player)
    {
        var text = Instantiate(playerInfoPrefab, Vector3.zero, Quaternion.identity);
        text.name = player.Joined.ToShortTimeString();
        text.GetComponent<TextMeshProUGUI>().text = player.Id + ":" + player.Data["PlayerLevel"].Value;
        var rectTramsform = text.GetComponent<RectTransform>();
        rectTramsform.SetParent(playerInfoContainer.transform);
    }
    
    private void ClearContainer()
    {
        if (playerInfoContainer is not null && playerInfoContainer.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in playerInfoContainer.transform)
            { 
                Destroy(VARIABLE.gameObject);
            }
        }
    }

    async void PollForLobbyUpdate()
    {
        _currentLobby.currentLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
        PopulateUIElements();
    }

    public async void ChangeLobbyNameButtonACT()
    {
        var newLobbyName = newName.text;

        try
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions();
            options.Name = newLobbyName;

            _currentLobby.currentLobby = await Lobbies.Instance.UpdateLobbyAsync(lobbyId, options);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
    public async void ChangePlayerButtonACT()
    {
        var playerLevel = newPlayerLevel.text;

        try
        {
            UpdatePlayerOptions options = new UpdatePlayerOptions();
            options.Data = new Dictionary<string, PlayerDataObject>()
            {
                { "PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerLevel) }
            };
            await LobbyService.Instance.UpdatePlayerAsync(lobbyId,  AuthenticationService.Instance.PlayerId, options);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    
}
