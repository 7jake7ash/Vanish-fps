using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    public GameObject spawn;

    public int team;

    void OnEnable()
    {
        if (PV.IsMine)
        {
            CreateController();

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Debug.LogError("onEnable");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    { 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        //if(PV.IsMine)
        //{
        //    CreateController();
        //}
    }

    void CreateController()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;

        Debug.LogError("Create controller");

        GameObject playerController = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
        
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            GetComponent<PhotonView>().RPC("Visibality", RpcTarget.AllBuffered, playerController.GetComponent<PhotonView>().ViewID);
        } else
        {
            GetComponent<PhotonView>().RPC("setTeam", RpcTarget.AllBuffered, playerController.GetComponent<PhotonView>().ViewID);
        }
    }
    [PunRPC]
    void Visibality(int id)
    {
        GameObject plrCon = PhotonNetwork.GetPhotonView(id).gameObject;

        plrCon.GetComponent<PlayerController>().plrManager = this;

        team = 0;

        GameObject head = plrCon.transform.Find("Recoil/CameraHolder/Head").gameObject;
        head.gameObject.GetComponent<MeshRenderer>().enabled = true;
        head.tag = "Dead";
        GameObject body = plrCon.transform.Find("Model/Body").gameObject;
        body.gameObject.GetComponent<MeshRenderer>().enabled = true;
        body.tag = "Dead";
        GameObject model = plrCon.transform.Find("Model/VanishPlayer/Cube").gameObject;
        model.GetComponent<MeshRenderer>().enabled = true;
        SetTag(model, "Dead");
        plrCon.transform.Find("Recoil/CameraHolder/itemContainer").gameObject.SetActive(true);
    }
    [PunRPC]
    void setTeam(int id)
    {
        GameObject plrCon = PhotonNetwork.GetPhotonView(id).gameObject;

        plrCon.GetComponent<PlayerController>().plrManager = this;

        //if(team > 0)
            //SetLayerRecursively(plrCon, team + 14);

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
