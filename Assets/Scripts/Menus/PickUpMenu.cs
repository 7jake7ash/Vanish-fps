using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PickUpMenu : MonoBehaviourPun
{
    public GameObject menu;
    
    
    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.CompareTag("Item") && photonView.IsMine)
        {
            menu.SetActive(true);
            //collider.transform.GetComponent<PickUp>().Model.GetComponent<Outline>().enabled = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject.CompareTag("Item") && photonView.IsMine)
        {
            menu.SetActive(false);
            //collider.transform.GetComponent<PickUp>().Model.GetComponent<Outline>().enabled = false;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if(!collider.gameObject.CompareTag("Item") && photonView.IsMine)
        {
            menu.SetActive(false);
        }
    }
}
