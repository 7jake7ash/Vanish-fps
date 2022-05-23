using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errortext;
    [SerializeField] TMP_Text roomNametext;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject joinRoomButton;
    [SerializeField] TMP_InputField RoomNameInput;
    [SerializeField] Button joinButton;

    private List<RoomInfo> roomList;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsConnected)
            return;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("login");
        Debug.Log("Joined Lobby");
    }

    public void OnLogin()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public void nameInput(string name)
    {
        joinButton.interactable = !string.IsNullOrEmpty(name);
    }
    public void FindRoom()
    {
        string name = RoomNameInput.text;
        Debug.LogError(name);
        PhotonNetwork.JoinRoom(name);
    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true
        };
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        MenuManager.Instance.OpenMenu("loading");
    }
    public void CreateRoomPrivate()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = false
        };
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.CurrentRoom.MaxPlayers = 10;
        
        PhotonNetwork.LoadLevel(1);
        
        Debug.Log("Joined Room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Failed to join");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errortext.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }


    public void JoinRoom(RoomInfo Info)
    {
        PhotonNetwork.JoinRoom(Info.Name);
    }


    public override void OnRoomListUpdate(List<RoomInfo> list)
    {
        //Debug.LogError(roomList.Count);
        roomList = list;
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        //for(int i = 0; i < roomList.Count; i++)
        //{
        //    //if(!roomList[i].IsVisible) {return;}
        //    if (roomList[i].RemovedFromList)
        //    {
        //        continue;
        //    }
        //    Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        //}
        foreach (RoomInfo a in roomList)
        {
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(a);
        }

        base.OnRoomListUpdate(roomList);
    }
}
