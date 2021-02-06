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
}
