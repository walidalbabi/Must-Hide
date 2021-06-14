using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FriendScript : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    public FriendInfo FriendInfo { get; private set; }

    [SerializeField]
    private Image onlineOffline;

    [SerializeField]
    private Sprite online, offline;



    public void SetRoomInfo(FriendInfo friendInfo)
    {
        FriendInfo = friendInfo;
        _text.text = friendInfo.UserId;

        if (friendInfo.IsOnline)
        {
            onlineOffline.sprite = online;
        }
        else
        {
            onlineOffline.sprite = offline;
        }
    }

    public void Invite()
    {
        if (FriendInfo != null)
        {
            VivoxManager.instance.Send_Direct_Message(FriendInfo.UserId, "inv" , "channel" + Photon.Pun.PhotonNetwork.AuthValues.UserId);
            Debug.Log("Invite Sended");
        }

    }

    public void Remove()
    {
        VivoxManager.instance.Send_Direct_Message(FriendInfo.UserId, "rmv", Photon.Pun.PhotonNetwork.AuthValues.UserId);
        PlayFabLogin.instance.RemoveFriends(FriendInfo.UserId);
        Destroy(gameObject);
    }
}
