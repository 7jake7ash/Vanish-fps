using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeamColor : MonoBehaviour
{
    public PlayerController plrControl;
    public Outline outline;
    public Outline headOutline;

    void Start()
    {
        Change();
    }

    void Update()
    {
        
    }

    public void Change()
    {
        if (plrControl.gameObject.GetComponent<PhotonView>().IsMine)
        {
            plrControl.gameObject.GetComponent<PhotonView>().RPC("changeColor", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void changeColor()
    {
        if (plrControl.plrManager.team == 2)
        {
            outline.OutlineColor = Color.blue;
            headOutline.OutlineColor = Color.blue;
        }
        else
        {
            outline.OutlineColor = Color.red;
            headOutline.OutlineColor = Color.red;
        }
    }
}
