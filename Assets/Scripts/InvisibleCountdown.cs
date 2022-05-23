using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InvisibleCountdown : MonoBehaviourPun
{
    public GameObject TimerObject;
    public AudioSource invisPickupClip;

    public int minWait;
    public int maxWait;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (other.GetComponent<PhotonView>().IsMine)
            {
                //Image
                TimerObject.SetActive(true);

                Photon.Pun.UtilityScripts.Timer timer = TimerObject.GetComponent<Photon.Pun.UtilityScripts.Timer>();

                timer.enabled = true;

                timer.image.SetActive(true);

                timer.SetVis(other.gameObject.GetComponent<PhotonView>().ViewID);

                //Audio
                photonView.RPC("PlayAudio", RpcTarget.All);
                //Constant Sound
                other.gameObject.GetComponent<PlayerController>().playSound();

                //Reset
                Invoke("resetPickup", Random.Range(minWait, maxWait));
            }
        }
    }

    [PunRPC]
    void PlayAudio()
    {
        invisPickupClip.Play();

        gameObject.SetActive(false);
    }

    [PunRPC]
    void resetPickup()
    {
        gameObject.SetActive(true);
    }
}
