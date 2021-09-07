using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviourPun
{
    public float Health = 100;

    public GameObject hitMarker;

    public int creditAmount;

    public Loot lootTable;

    public void Shot(float damage)
    {
        Health -= damage;

        Debug.Log(Health);

        if (Health <= 0)
        {
            photonView.RPC("drop", RpcTarget.All);

            Health = 100;
        }
    }

    public void HitMarker(float time)
    {
        Invoke("Wait", time);
    }

    [PunRPC]
    public void drop()
    {
        GameObject Coins = PhotonNetwork.Instantiate("Coins", transform.position, Quaternion.identity);
        Coins.GetComponent<CreditAmount>().credit = creditAmount;

        Destroy(gameObject);
    }

    public void Wait()
    {
        hitMarker.SetActive(false);
    }
}

