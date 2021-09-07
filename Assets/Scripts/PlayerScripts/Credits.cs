using System.Collections;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using UnityEngine;

public class Credits : MonoBehaviourPun
{
    public int credits = 0;

    public TMP_Text numberText;

    public GameObject hit;

    public HealthBar health;

    bool canBuy;

    void Start()
    {
        health = GetComponent<HealthBar>();
        
        if(photonView.IsMine)
            numberText.gameObject.SetActive(true);
            canBuy = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coins"))
        {
            if (!health.Dead)
            {
                hit = other.transform.parent.gameObject;

                photonView.RPC("DestroyCoin", RpcTarget.All);

                if (photonView.IsMine)
                    photonView.RPC("AddCredits", RpcTarget.All, other.transform.parent.GetComponent<CreditAmount>().credit);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (photonView.IsMine && other.CompareTag("ShopItem") && canBuy)
        {
            ShopItem shop = other.transform.parent.GetComponent<ShopItem>();

            if (credits >= shop.Price && Input.GetKey(KeyCode.F))
            {
                canBuy = false;

                PhotonNetwork.Instantiate(shop.prefab.name, shop.transform.position, Quaternion.identity);

                photonView.RPC("AddCredits", RpcTarget.All, -shop.Price);

                Invoke("Cooldown", 1);
            }
        }
    }

    public void Cooldown()
    {
        canBuy = true;
    }

    [PunRPC]
    public void AddCredits(int amount)
    {
        credits = credits + amount;

        numberText.text = credits.ToString();
    }

    [PunRPC]
    public void DestroyCoin()
    {
        Destroy(hit);
    }
}
