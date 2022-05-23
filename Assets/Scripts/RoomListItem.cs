using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] public TMP_Text text;
    [SerializeField] public TMP_Text plrCount;
    [SerializeField] public GameObject lockIcon;

    public RoomInfo Info;
    
    public void SetUp(RoomInfo _Info)
    {
        Info = _Info;
        text.text = _Info.Name;
        
        if(Info.IsOpen)
        {
            text.color = Color.black;
            plrCount.color = Color.black;
        }
        else
        {
            text.color = Color.red;
            plrCount.color = Color.red;
        }

        lockIcon.SetActive(!Info.IsOpen);
        GetComponent<Button>().interactable = Info.IsOpen;


        plrCount.SetText(Info.PlayerCount + " / " + Info.MaxPlayers);
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(Info);
    }
}
