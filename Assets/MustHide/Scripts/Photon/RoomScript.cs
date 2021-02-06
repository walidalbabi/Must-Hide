using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomScript : MonoBehaviour
{

    [SerializeField]
    private Text _text;

    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _text.text = roomInfo.MaxPlayers + ", " + roomInfo.Name;
    }

    public void OnClick_Button()
    {
        NetworkManager.instance.JoinRoom(RoomInfo.Name);
    }

}
