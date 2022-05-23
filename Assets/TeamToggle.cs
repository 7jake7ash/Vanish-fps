using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamToggle : MonoBehaviourPun
{
    public GameObject teamBlock;
    public GameObject teamUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle(bool teams)
    {
        photonView.RPC("UI", RpcTarget.All, teams);
    }

    [PunRPC]
    void UI(bool t)
    {
        //teamUI.SetActive(t);
        teamBlock.SetActive(!t);

        foreach (PhotonView plr in PhotonNetwork.PhotonViewCollection)
        {
            if (plr.gameObject.CompareTag("Player"))
            {
                if (t && plr.GetComponent<PlayerController>().plrManager.team == 0)
                {
                    plr.GetComponent<PlayerController>().plrManager.team = Random.Range(1, 2);
                }
                else
                {
                    plr.GetComponent<PlayerController>().plrManager.team = 0;
                }
            }
        }
    }
}
