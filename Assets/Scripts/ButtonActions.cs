using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class ButtonActions : MonoBehaviour
{  [SerializeField] TextMeshProUGUI _Text;
    private NetworkManager _networkManager;
    // Start is called before the first frame update
    void Start()
    {
        _networkManager = GetComponentInParent<NetworkManager>();
    }

   public void StartHost()
    {
        _networkManager.StartHost();
        InitMovementText();
    }

    public void StartClient()
    {
        _networkManager.StartClient();
        InitMovementText();
    }

    public void SubmitNewPosition()
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject(); //Objeyi bul
        var player = playerObject.GetComponent<PlayerMovement>(); //oyuncuyu bul
        player.Move(); //Fonksiyonu kullan
    }

    private void InitMovementText()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            _Text.text = "Move";
        }
        else if(NetworkManager.Singleton.IsClient)
        {
            _Text.text = "RequestMove";
        }
    }
}
