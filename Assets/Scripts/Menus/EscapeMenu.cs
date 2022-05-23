using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class EscapeMenu : MonoBehaviourPunCallbacks
{
    public static EscapeMenu Instance;

    public bool EscapeOpen;

    [SerializeField] private GameObject menu;

    [Header("Escape")]
    [SerializeField] private GameObject escapeMenu;

    [Header("Options")]
    [SerializeField] private GameObject optionsMenu;
    void Start()
    {
        Instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(!menu.activeSelf);
            optionsMenu.SetActive(false);

            EscapeOpen = menu.activeSelf;

            if (EscapeOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                escapeMenu.SetActive(true);
            }
        }
    }

    public void Options()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
        escapeMenu.SetActive(!escapeMenu.activeSelf);
    }

    public void LeaveRoom()
    {
        //Destroy(RoomManager.Instance.gameObject);
        Debug.Log("left");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }


    public override void OnLeftRoom()
    {

    }
}
