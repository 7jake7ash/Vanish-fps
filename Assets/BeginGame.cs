using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeginGame : MonoBehaviourPun
{
    static bool anyPressed;
    bool isPressed;
    public string scene;
    
    public Photon.Pun.UtilityScripts.CountdownTimer countdownTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        anyPressed = false;
        isPressed = false;
        
        countdownTimer.enabled = true;

        if (scene != null) 
        {
            transform.Find("Model/LevelText").GetComponent<TMP_Text>().text = scene;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!anyPressed && other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {   
            if (PhotonNetwork.IsMasterClient)
            {
                isPressed = true;
                anyPressed = true;
                    
                countdownTimer.SetStartTime();

                PhotonNetwork.CurrentRoom.IsOpen = false;

                photonView.RPC("UI", RpcTarget.All);

                Debug.Log("start");
            }
        }
    }

    public void StartGame()
        {
            if(isPressed)
            {
                //photonView.RPC("destroy", RpcTarget.MasterClient);
                PhotonNetwork.LoadLevel(countdownTimer.index);
            }
        }
}
