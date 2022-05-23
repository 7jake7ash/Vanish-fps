using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    void Start()
    {  
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    }

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    Debug.LogError(SceneManager.GetActiveScene().buildIndex);
    //    if (SceneManager.GetActiveScene().buildIndex >= 1)
    //    {
    //        Debug.LogError("enable");
    //        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    //    }
        
        //SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    //}
    
    
    //private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    //{
    //    if(scene.buildIndex >= 1)
    //    {                           
    //        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    //    }
    //}
}
