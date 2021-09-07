using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class StartGunSpawn : MonoBehaviourPun
{
    [Header("PickUp")]
    public GameObject StartGun;

    void Start()
    {
        PhotonNetwork.InstantiateRoomObject(StartGun.name, transform.position, Quaternion.identity);
    }
}
