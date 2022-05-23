using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class playerSpawn : MonoBehaviourPun
{
    int players;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("PlrMan").Length == PhotonNetwork.PlayerList.Length)
            return;
        
        GameObject plrMan = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        photonView.RPC("dontDestroy", RpcTarget.AllBuffered, plrMan.GetComponent<PhotonView>().ViewID);
        Debug.LogError("player joined");

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void dontDestroy(int id)
    {
        GameObject plrMan = PhotonNetwork.GetPhotonView(id).gameObject;
        DontDestroyOnLoad(plrMan);
    }
}
