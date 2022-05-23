using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapScreen : MonoBehaviourPun
{
    public GameObject canvas;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine && PhotonNetwork.IsMasterClient)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            canvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player") && PhotonNetwork.IsMasterClient)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            canvas.SetActive(false);
        }
    }
}
