using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class StartGame : MonoBehaviourPun
{
    public CountdownTimer timer;

    public TMP_Text text;

    public GameObject countdown;

    public float Timer;
    bool GameStarted;
    void Start()
    {
        //gameObject.SetActive(PhotonNetwork.IsMasterClient);

        timer = GetComponent<CountdownTimer>();

        GameStarted = false;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                timer.SetStartTime();

                photonView.RPC("SetUp", RpcTarget.All);
            }
        }
    }

    public void startMatch()
    {
        Debug.LogWarning("Start Match");

        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    public void SetUp()
    {
        //Timer
        Timer = 10;

        text.enabled = true;
        GameStarted = true;
        countdown.SetActive(true);
    }
    [PunRPC]
    public void TimerCount()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
            text.text = Mathf.Round(Timer).ToString();
        }
        else
        {
            startMatch();

            GameStarted = false;
            text.enabled = false;
            countdown.SetActive(false);
        }
    }
}
