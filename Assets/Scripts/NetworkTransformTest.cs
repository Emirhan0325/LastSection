using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    
    void Update()
    {
        if (IsOwner && IsServer)
        {
            transform.RotateAround(GetComponentInParent<Transform>().position,Vector3.up, 100*Time.deltaTime);
        }
    }
}
