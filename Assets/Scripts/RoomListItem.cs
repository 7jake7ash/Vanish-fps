using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] public TMP_Text text;
    [SerializeField] public TMP_Text plrCount;

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
        
        plrCount.SetText(Info.PlayerCount + " / " + Info.MaxPlayers + "  Game Started: " + !Info.IsOpen);
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(Info);
    }
}
