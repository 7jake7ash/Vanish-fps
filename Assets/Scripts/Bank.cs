using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bank : MonoBehaviourPun
{
    private int storedCredits;
    
    public Credits credits;

    void Start()
    {
        credits = GetComponent<Credits>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bank"))
        {
            photonView.RPC("Safe", RpcTarget.All);

            if(photonView.IsMine)
                other.transform.Find("Text").GetComponent<CreditAmount>().Add(storedCredits);
        }
    }
    [PunRPC]
    public void Safe()
    {
        //Store Credits
        storedCredits += credits.credits;
        //Reset
        credits.credits = 0;
        //Update text
        credits.numberText.text = credits.numberText.text = credits.credits.ToString();

        Debug.Log(storedCredits);


    }
}
