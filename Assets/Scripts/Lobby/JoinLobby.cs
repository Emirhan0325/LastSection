using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class JoinLobby : MonoBehaviour
{
    public TMP_InputField _InputField;

    public async void JoinLobbyWithLobbyCode(string lobbyCode)
    {
        var code = _InputField.text;
        try // try catch ile yapmamızın sebebi denesin eğer olmazsa logError versin
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            options.Player = new Player(AuthenticationService.Instance.PlayerId);
            options.Player.Data = new Dictionary<string, PlayerDataObject>()
            {
                { "PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "1") }
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code,options);
            DontDestroyOnLoad(this);
            GetComponent<CurrentLobby>().currentLobby = lobby;
            
            LobbyStatic.LogPlayerInLobby(lobby);
            LobbyStatic.LoadLobbyRoom();
            
            Debug.Log("Joined Lobby With Code:" + code);
        }
        catch (LobbyServiceException e) // bu şekilde doğru hata alırız
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async void JoinLobbyWithLobbyId(string lobbyId)
    {
        try // try catch ile yapmamızın sebebi denesin eğer olmazsa logError versin
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();
            options.Player = new Player(AuthenticationService.Instance.PlayerId);
            options.Player.Data = new Dictionary<string, PlayerDataObject>()
            {
                { "PlayerLevel", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "1") }
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId,options);
            Debug.Log("Joined Lobby With Id:"+ lobbyId);
            Debug.LogWarning("Lobby Code :"+ lobby.LobbyCode);
            
            DontDestroyOnLoad(this);
            GetComponent<CurrentLobby>().currentLobby = lobby;
            
            LobbyStatic.LogPlayerInLobby(lobby);
            LobbyStatic.LoadLobbyRoom();
        }
        catch (LobbyServiceException e) // bu şekilde doğru hata alırız
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async void QuickJoinMethod()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            
            DontDestroyOnLoad(this);
            GetComponent<CurrentLobby>().currentLobby = lobby;
            
            Debug.Log("Joined Lobby With Quick Join:"+ lobby.Id);
            Debug.LogWarning("Lobby Code :"+ lobby.LobbyCode);
            
            LobbyStatic.LoadLobbyRoom();
        }
        catch (LobbyServiceException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
