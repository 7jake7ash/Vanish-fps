using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TeamSelect : MonoBehaviourPun
{
    public int team;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController plrController = other.GetComponent<PlayerController>();
            plrController.plrManager.team = team;

            photonView.RPC("TeamChange", RpcTarget.AllBuffered, plrController.GetComponent<PhotonView>().ViewID);
            plrController.GetComponent<TeamColor>().Change();
        }
    }
    [PunRPC]
    void TeamChange(int id)
    {
        PhotonNetwork.GetPhotonView(id).GetComponent<PlayerController>().plrManager.team = team;
    }
}
