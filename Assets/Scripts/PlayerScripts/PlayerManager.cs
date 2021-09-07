using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviourPun
{
    PhotonView PV;

    public GameObject spawn;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    
    void Update()
    {
        
    }

    void CreateController()
    {
        GameObject playerController = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            photonView.RPC("Visibality", RpcTarget.AllBuffered, playerController.GetComponent<PhotonView>().ViewID);
        }
    }
    [PunRPC]
    void Visibality(int id)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            GameObject plrCon = PhotonNetwork.GetPhotonView(id).gameObject;
            GameObject head = plrCon.transform.Find("Recoil/CameraHolder/Head").gameObject;
            head.gameObject.GetComponent<MeshRenderer>().enabled = true;
            head.tag = "Dead";
            GameObject model = plrCon.transform.Find("Model").gameObject;
            model.SetActive(true);
            SetTag(model, "Dead");
            plrCon.transform.Find("Recoil/CameraHolder/itemContainer").gameObject.SetActive(true);
        }
    }

    void SetTag(GameObject obj, string tag)
    {
        if (null == obj)
        {
            return;
        }

        obj.tag = tag;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetTag(child.gameObject, tag);
        }
    }
}
