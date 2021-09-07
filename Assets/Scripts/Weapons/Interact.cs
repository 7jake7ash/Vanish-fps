using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviourPun
{
    [Header("Item Prefab")]
    public GameObject prefab;

    public Gun gunScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public float dropForwardForce, dropUpwardForce;
    
    [SerializeField] public PhotonRigidbodyView rbView;

    private Transform Cam;

    private Transform player;

    public bool equipped;
    static bool slotFull;

    private bool inTrigger = false;

    void Awake()
    {
        rbView = GetComponent<PhotonRigidbodyView>();
    }

    void Start()
    {
        slotFull = false;

        //SetUp
        if (!equipped)
        {
            gunScript.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
            //slotFull = false;
        }
        if (equipped)
        {
            gunScript.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }
    }

    private void Update()
    {   
        if (inTrigger)
        {
            //if (transform.IsChildOf(player.Find("CameraHolder/itemContainer")))
            //{
            //    rbView.enabled = false;
            //    if (photonView.IsMine)
            //    {
            //        slotFull = true;
            //        Debug.Log(slotFull);
            //    }
            //}
            //else
            //{
            //    if (photonView.IsMine)
            //    {
            //        slotFull = false;
            //        Debug.Log(slotFull);
            //    }
            //}

            if (player.GetComponent<PhotonView>().IsMine)
            {
                if (!equipped && !slotFull && Input.GetKeyDown(KeyCode.F))
                {
                    if (photonView.Owner.UserId == player.GetComponent<PhotonView>().Owner.UserId)
                    {
                        PickUp();

                        gunScript.ammunitionDisplay.enabled = true;
                    }
                    else
                    {
                        photonView.TransferOwnership(player.GetComponent<PhotonView>().Controller);

                        PickUp();

                        gunScript.ammunitionDisplay.enabled = true;
                    }
                }

                if (equipped && Input.GetKeyDown(KeyCode.Q))
                {
                    Drop();
                    gunScript.ammunitionDisplay.enabled = false;

                    SetLayerRecursively(gameObject, 11);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inTrigger = true;
            player = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inTrigger = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inTrigger = false;
        }
    }
    
    public void PickUp()
    {
        SetLayerRecursively(gameObject, 14);

        PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);

        if(photonView.IsMine)
            slotFull = true;

        //Send To All Clients
        //photonView.RPC("Equip", RpcTarget.AllBuffered);
    }
    public void Drop()
    {
        if (photonView.IsMine)
            slotFull = false;

        //Send To All Clients
        photonView.RPC("UnEquip", RpcTarget.All);

        //momentum
        rb.velocity = player.GetComponent<CharacterController>().velocity;

        //AddForce
        rb.AddForce(Cam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(Cam.forward * dropUpwardForce, ForceMode.Impulse);

        //Add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);
    }

    public void OtherDrop()
    {
        if (photonView.IsMine)
            slotFull = false;

        //Send To All Clients
        photonView.RPC("UnEquip", RpcTarget.All);
    }

    [PunRPC]
    public void Equip()
    {
        rbView.enabled = false;

        equipped = true;

        Invoke("waitForOwner", 0.2f);
    }
    [PunRPC]
    public void UnEquip()
    {
        rbView.enabled = true;

        equipped = false;

        //Cam SetUp
        Cam = transform.parent.parent.Find("Cam").transform;

        //Set Parent to Null
        transform.SetParent(null);

        gunScript.enabled = false;

        rb.isKinematic = false;
        coll.isTrigger = false;

        photonView.TransferOwnership(-1);
    }
    [PunRPC]
    void waitForOwner()
    {
        transform.SetParent(player.Find("Recoil/CameraHolder/itemContainer").transform);

        rb.isKinematic = true;
        coll.isTrigger = true;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        gunScript.enabled = true;
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
}
