using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RescueShip : MonoBehaviourPun
{
    public GameObject countdown;
    public int addPlrs;
    public int plrsLeft;
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
        if (other.gameObject.CompareTag("Player"))
        {
            addPlrs = 1;
            
            photonView.RPC("AddPlayer", RpcTarget.All);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            addPlrs = -1;

            photonView.RPC("AddPlayer", RpcTarget.All);
        }
    }
    public void Reset()
    {
        addPlrs = -plrsLeft;
        photonView.RPC("AddPlayer", RpcTarget.All);
    }

    [PunRPC]
    public void AddPlayer()
    {
        plrsLeft += addPlrs;
        Debug.LogWarning(plrsLeft);
        addPlrs = 0;
    }
}
