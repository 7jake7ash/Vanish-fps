using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviourPun
{
    public float Health = 100;

    static int playersDead;

    public Vector3 spawn;
    public bool Dead = false;
    public LayerMask layer;
    public PlayerController playerController;
    public AudioSource hitSound;

    //Gun
    Transform itemCont;
    public BoxCollider coll;

    [SerializeField] RectTransform rect;

    public GameObject winScreen;

    public TMP_Text plrWonText;
    public GameObject hitMarker;

    Vector3 PlrPos;

    void Start()
    {
        itemCont = transform.Find("Recoil/CameraHolder/itemContainer");
        playerController = GetComponent<PlayerController>();
        if(photonView.IsMine)
            Dead = false;
    }

    public void Shot(float damage)
    {
        Health -= damage;
        
        Debug.Log(Health);

        hitSound.Play();

        //Death
        if(Health <= 0)
        {
            if(itemCont.transform.childCount != 0)
            { 
                Gun gun = itemCont.GetComponentInChildren<Gun>();
                gun.DropGun();

                gun.DestroyGun();
            }

            if (photonView.IsMine)
            {
                Dead = true;
            }

            Health = 100;

            PlrPos = transform.position;

            photonView.RPC("death", RpcTarget.All);

            if (photonView.IsMine)
                Dead = false;
        }
    }
    
    [PunRPC]
    public void Destroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void death()
    {
        playersDead++;

        //gameObject.SetActive(false);
        Transform head = transform.Find("Recoil/CameraHolder/Head");
        head.gameObject.GetComponent<MeshRenderer>().enabled = false;
        transform.Find("Model").gameObject.SetActive(false);

        gameObject.tag = "Dead";
        head.tag = "Dead";
        transform.Find("Model/Body").tag = "Dead";

        //Find Teams
        int team0 = 0;
        int team1 = 0;
        int team2 = 0;

        foreach (PhotonView plr in PhotonNetwork.PhotonViewCollection)
        {
            if (plr.gameObject.CompareTag("Player"))
            {
                if(plr.GetComponent<PlayerController>().plrManager.team == 0)
                {
                    team0++;
                }
                if(plr.GetComponent<PlayerController>().plrManager.team == 1)
                {
                    team1++;
                }
                if (plr.GetComponent<PlayerController>().plrManager.team == 2)
                {
                    team2++;
                }
            }
        }
        //One Player Left
        if (team0 == 1 && team1 == 0 && team2 == 0)
        {
            playersDead = 0;
            Debug.LogError("Game Over");
            GameObject playerWon = GameObject.FindGameObjectWithTag("Player");
            plrWonText.text = "Winner: " + playerWon.GetComponent<PhotonView>().Owner.NickName;
            winScreen.gameObject.SetActive(true);
            Invoke("StopGame", 7f);
        }
        //Other Team Dead
        else if(team1 > 0 && team2 == 0 || team2 > 0 && team1 == 0)
        {
            playersDead = 0;
            Debug.LogError("Game Over");
            GameObject playerWon = GameObject.FindGameObjectWithTag("Player");
            plrWonText.text = "Winner: " + playerWon.GetComponent<PhotonView>().Owner.NickName;
            winScreen.gameObject.SetActive(true);
            Invoke("StopGame", 7f);
        }
    }
    
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void HitMarker(float time)
    {
        //Hit Sound
        
        if (Health <= 0)
        {
            hitMarker.SetActive(true);

            Invoke("Wait", time);
        }
    }

    public void Wait()
    {
        hitMarker.SetActive(false);
    }

    void StopGame()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }
}
