using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    private string PlayerID; // oyuna girerken bize bir id atanacak 
    private RelayHostData _hostData;
    private RelayJoinData _joinData;
    public TextMeshProUGUI IdText;
    public TextMeshProUGUI JCText;
    public TMP_InputField inputField;
    public TMP_Dropdown PlayerCount;
    
    async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log("Unity Services Init");
        SignIn();
    }         public string JoinCode;


    async void SignIn()
    {
        Debug.Log("Signing in");
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        PlayerID = AuthenticationService.Instance.PlayerId; // benim yazdığım string olan PlayerID değil. Giriş yaptığımızda ki ananim ID mizi Benim PlayerID me eşitliyoruz
        Debug.Log("Signed in" + PlayerID);
        IdText.text = PlayerID;
    }

     public async void OnHostClick()
    {
        int maxPlayerCount = Convert.ToInt32(PlayerCount.options[PlayerCount.value].text);

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayerCount);

        _hostData = new RelayHostData()
        {
            IPv4Adress = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,

            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            Key = allocation.Key,
        };

        _hostData.JoinCode = await RelayService.Instance.GetJoinCodeAsync(_hostData.AllocationID); // odanın ID si
        Debug.LogWarning("Join Code :"+ _hostData.JoinCode);
        
        Debug.Log("allocate complete :"+ _hostData.AllocationID);
        JCText.text = _hostData.JoinCode;

        UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
        
        transport.SetRelayServerData(_hostData.IPv4Adress,_hostData.Port,_hostData.AllocationIDBytes,_hostData.Key,_hostData.ConnectionData);
        NetworkManager.Singleton.StartHost();
    }
     
     public async void OnJoinClick()
     {
         JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(inputField.text);

         _joinData = new RelayJoinData()
         {
             IPv4Adress = allocation.RelayServer.IpV4,
             Port = (ushort)allocation.RelayServer.Port,

             AllocationID = allocation.AllocationId,
             AllocationIDBytes = allocation.AllocationIdBytes,
             ConnectionData = allocation.ConnectionData,
             HostConnectionData = allocation.HostConnectionData,
             Key = allocation.Key,
         };
         Debug.Log("Join Succes :"+ _joinData.AllocationID);
         
         UnityTransport transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
        
         transport.SetRelayServerData(_joinData.IPv4Adress,_joinData.Port,_joinData.AllocationIDBytes,_joinData.Key,_joinData.ConnectionData,_joinData.HostConnectionData);
         NetworkManager.Singleton.StartClient();
     }
    
     public struct RelayHostData
     {
         public string JoinCode;
         public string IPv4Adress;
         public ushort Port;
         public Guid AllocationID;
         public byte[] AllocationIDBytes;
         public byte[] ConnectionData;
         public byte[] Key;
     }
     
     public struct RelayJoinData
     {
         public string IPv4Adress;
         public ushort Port;
         public Guid AllocationID;
         public byte[] AllocationIDBytes;
         public byte[] ConnectionData;
         public byte[] HostConnectionData; 
         public byte[] Key;
     }
     
    
}
