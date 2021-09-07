using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviourPunCallbacks
{
    [Header("Prefab")]
    public GameObject prefab;
    float throwForce = 5;

    private GameObject newPrefab;
    private Transform Container;
    private PhotonView playerView;
    private Collider otherColl;
    private bool inTrigger = false;
    public bool canEquip = false;
    public static bool SlotFull = false;

    void Start()
    {
        Invoke("Equip", 1);
    }

    void Update()
    {
        if (inTrigger && Input.GetKeyDown(KeyCode.F) && canEquip)
        {
            canEquip = false;

            playerView = otherColl.gameObject.GetComponent<PhotonView>();
            if (playerView.IsMine && !SlotFull)
            {
                Container = otherColl.transform.Find("Recoil/CameraHolder/itemContainer");
                if (Container.childCount <= 1)
                {
                    SlotFull = true;

                    newPrefab = PhotonNetwork.Instantiate(prefab.name, transform.position, Quaternion.identity);
        
                    if (Container.childCount == 1)
                    {
                        photonView.RPC("Spawn", RpcTarget.MasterClient, otherColl.gameObject.GetComponent<PhotonView>().ViewID);
                        PhotonNetwork.Destroy(Container.GetChild(0).gameObject);
                    }
                    //SetParent
                    photonView.RPC("SetParent", RpcTarget.AllBuffered, newPrefab.GetComponent<PhotonView>().ViewID, playerView.ViewID);
                    newPrefab.GetComponent<Gun>().Parent(newPrefab.GetComponent<PhotonView>().ViewID, playerView.ViewID);

                    photonView.RPC("Destroy", RpcTarget.MasterClient);
                    SlotFull = false;
                }
            }
            canEquip = true;
        }
    }

    void Equip()
    {
        canEquip = true;
    }
    public void Drop()
    {
        photonView.RPC("Spawn", RpcTarget.MasterClient, otherColl.gameObject.GetComponent<PhotonView>().ViewID);
    }
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.GetComponent<PhotonView>().IsMine)
        {
            inTrigger = true;
            otherColl = other;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.GetComponent<PhotonView>().IsMine)
        {
            inTrigger = false;
        }
    }

    [PunRPC]
    public void SetParent(int viewId, int plrId)
    {
        GameObject prefab = PhotonNetwork.GetPhotonView(viewId).gameObject;
        Transform container = PhotonNetwork.GetPhotonView(plrId).transform.Find("Recoil/CameraHolder/itemContainer");
        prefab.gameObject.GetComponent<Gun>().Container = container;
        //prefab.transform.Find("Anchor/Design").gameObject.SetActive(!photonView);
        prefab.transform.SetParent(container);
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
    }
    [PunRPC]
    public void Destroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    public void Spawn(int id)
    {
        otherColl = PhotonNetwork.GetPhotonView(id).GetComponent<Collider>();
        Transform gun = otherColl.transform.Find("Recoil/CameraHolder/itemContainer").GetChild(0);
        GameObject pickup = PhotonNetwork.InstantiateRoomObject(gun.GetComponent<Gun>().pickupPrefab.name, otherColl.transform.position + otherColl.transform.forward * 1.1f, Quaternion.Euler(0,otherColl.transform.rotation.eulerAngles.y,0));
        Rigidbody Rb = pickup.GetComponent<Rigidbody>();
        Rb.AddForce(otherColl.transform.forward * throwForce, ForceMode.Impulse);
        Rb.AddTorque(otherColl.transform.forward + new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), ForceMode.Impulse);
    }
}
