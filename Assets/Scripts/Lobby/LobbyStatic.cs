using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LobbyStatic
{
    public static void LogPlayerInLobby(Lobby lobby)
    {
        foreach (Player player in lobby.Players) //
        {
            Debug.Log("Player ID :"+ player.Id);
            Debug.Log("Player Level" + player.Data["PlayerLevel"].Value);
        }
    }
    
    public static void LogLobby(Lobby lobby)
    {
        Debug.Log("Lobby ID : "+ lobby.Id + "\n" +
                  lobby.Data["GameMode"].Value);
    }

    public static void LoadLobbyRoom()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
