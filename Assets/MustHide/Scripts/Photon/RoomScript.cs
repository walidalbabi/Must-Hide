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

    private void Start()
    {
        StartCoroutine(RefreshUI());
    }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _text.text =   "House      "+ roomInfo.Name.Substring(0 , 5) + "            " + roomInfo.PlayerCount+"/"+roomInfo.MaxPlayers;
    }

    IEnumerator RefreshUI()
    {
        SetRoomInfo(RoomInfo);
        yield return new WaitForSeconds(2f);
    }

    public void OnClick_Button()
    {
        NetworkManager.instance.JoinRoom(RoomInfo.Name);
    }

}
