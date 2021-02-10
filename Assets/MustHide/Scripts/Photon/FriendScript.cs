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

    public void SetRoomInfo(FriendInfo friendInfo)
    {
        FriendInfo = friendInfo;
        _text.text = friendInfo.UserId +"/"+friendInfo.IsOnline;
    }

    public void Invite()
    {
        if (FriendInfo != null)
        {
            VivoxManager.instance.Send_Direct_Message(FriendInfo.UserId, "channel" + Photon.Pun.PhotonNetwork.AuthValues.UserId);
            Debug.Log("Invite Sended");
        }

    }

    public void Remove()
    {
        PlayFabLogin.instance.RemoveFriends(FriendInfo.UserId);
        Destroy(gameObject);
    }
}
