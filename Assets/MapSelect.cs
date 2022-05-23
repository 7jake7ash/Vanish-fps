using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class MapSelect : MonoBehaviourPun
{
    public int mapIndex;

    static bool anyPressed;
    bool isPressed;
    public string scene;
    
    public Photon.Pun.UtilityScripts.CountdownTimer countdownTimer;

    void Start()
    {
        //countdownTimer.enabled = true;
        
        transform.Find("Text").GetComponent<TMP_Text>().text = scene;
    }

    public void OnClick()
    {
        transform.parent.parent.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        countdownTimer.enabled = true;

        PhotonNetwork.LoadLevel(mapIndex);

        countdownTimer.index = mapIndex;

        //countdownTimer.SetStartTime();

        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
}
