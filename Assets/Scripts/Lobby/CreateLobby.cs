using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobby : MonoBehaviour
{
    public TMP_InputField lobbyname;
    public TMP_InputField lobbycode;
    public TMP_Dropdown maxplayer;
    public TMP_Dropdown gamemode;
    public Toggle isLobbyPrivate;

    public async void CreateLobbyMethod()
    {
        string lobbyName = lobbyname.text;
        int maxPlayer = Convert.ToInt32(maxplayer.options[maxplayer.value].text);
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = isLobbyPrivate.isOn;
        options.Player = new Player(AuthenticationService.Instance.PlayerId);
        options.Player.Data = new Dictionary<string, PlayerDataObject>()
        {
            { "PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "3") }
        };

        options.Data = new Dictionary<string, DataObject>()
        {
            {
                "GameMode",
                new DataObject(DataObject.VisibilityOptions.Public, gamemode.options[gamemode.value].text,DataObject.IndexOptions.S1) // DATAOBJECT VE INDEXOPTİONS A BAK   
            }
        };
        
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, options);
        DontDestroyOnLoad(this);
        GetComponent<CurrentLobby>().currentLobby = lobby;
        Debug.Log("create lobby done");
        LobbyStatic.LogPlayerInLobby(lobby);
        LobbyStatic.LogLobby(lobby);
        lobbycode.text = lobby.LobbyCode;
        
        StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15f)); 
        
        LobbyStatic.LoadLobbyRoom();
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyID, float waitForSeconds)
    {
        var delay = new WaitForSeconds(waitForSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyID);
            yield return delay;
        }
    }

    
}
