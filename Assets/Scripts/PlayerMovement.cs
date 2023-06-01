using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    void Update()
    {
        transform.position = position.Value; // Bu sayede bizim position değerimiz update methodu içinde sürekli olarak position değiştirini değiştirecek.
    }

    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var randomPosition = GetRandomPositionOnPlane();
            transform.position = randomPosition; // Bu position sadece bizim bilgisayarımızda geçerli fakat bizim network dede değer vermemiz gerekiyor
            position.Value = randomPosition; // bu şekilde position değerimizi network değerine aktarmış olduk
        }
        else
        {
            SubmitPositionRequestServerRpc();
        }
    }
    
    
    // Hosttan istememiz lazım ki cilent dan bağlanan da hereket edebilsin bu yüzden Rpc tagli fonksiyon yapıp istediğimiz position değerini verdik
    [ServerRpc]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        position.Value = GetRandomPositionOnPlane();
    }
    static Vector3 GetRandomPositionOnPlane()
    {
        return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
    }
}
