using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Mathematics;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

public class GetLobbies : MonoBehaviour
{
    public GameObject buttonsContainer;
    public GameObject buttonPrefab;
    
    
    async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void GetLobbiesTest()
    {
        ClearContainer();
        try
        {
            QueryLobbiesOptions options = new();
            Debug.LogWarning("QueryLobbiesTest");
            options.Count = 25;
            
            // Filter for open Lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0",QueryFilter.OpOptions.GT),
                // new QueryFilter(QueryFilter.FieldOptions.S1,"Death Match",QueryFilter.OpOptions.EQ)
                //new QueryFilter() diyip yeni options ekleyebiliriz.
            };
            
            // Order by newest Lobbies first
            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(true, QueryOrder.FieldOptions.Created)
                //asc : true is en son açılan lobileri göster false ise ilk açılan lobileri göster 
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
            Debug.LogWarning("Get Lobbies Done Count :"+ lobbies.Results.Count);
            
            // burası sorunlu 
            foreach (Lobby bulunanLobby in lobbies.Results)
            {
                // Debug.Log("lobby ismi :"+ bulunanLobby.Name+"\n" +
                //           "Lobby oluşturulma tarihi"+bulunanLobby.Created+"\n"+
                //           "Lobby Code ="+bulunanLobby.LobbyCode+"\n"+
                //           "Lobby ID :"+bulunanLobby.Id);
                LobbyStatic.LogLobby(bulunanLobby);
                CreateLobbyButton(bulunanLobby);
            }
            // GetComponent<JoinLobby>().JoinLobbyWithLobbyId(lobbies.Results[0].Id); --- lobby e bağlanmayı sağlar
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void CreateLobbyButton(Lobby lobby)
    {
        var button = Instantiate(buttonPrefab, Vector3.zero, quaternion.identity); // butonu oluştur
        button.name = lobby.Name + "_Button"; // buton adını yazdır
        button.GetComponentInChildren<TextMeshProUGUI>().text = lobby.Name;
        var recTransform = button.GetComponent<RectTransform>();
        recTransform.SetParent(buttonsContainer.transform);
        button.GetComponent<Button>().onClick.AddListener(delegate { Lobby_OnClick(lobby); });
    }

    private void Lobby_OnClick(Lobby lobby)
    {
        Debug.Log("Clicked lobby : "+ lobby.Name);
        GetComponent<JoinLobby>().JoinLobbyWithLobbyId(lobby.Id);
    }

    private void ClearContainer()
    {
        if (buttonsContainer is not null && buttonsContainer.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in buttonsContainer.transform)
            {
                Destroy(VARIABLE.gameObject);
            }
        }
    }
    
    
    
    
    
    
}
