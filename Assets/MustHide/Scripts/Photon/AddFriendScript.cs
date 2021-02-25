using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AddFriendScript : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    public string senderID;

    private void Start()
    {
        Destroy(gameObject, 30f);
    }
    public void SetInvitationInfo(string senderName, string userID)
    {
        senderID = userID;
        _text.text = senderName + " Sended You a Friend Request";
    }

    public void Accept()
    {
        VivoxManager.instance.Send_Direct_Message(senderID, "acc", Photon.Pun.PhotonNetwork.AuthValues.UserId);

        PlayFabLogin.instance.AddFriends(senderID);
        Destroy(gameObject);
    }


    public void Reject()
    {
        Destroy(gameObject);
    }
}
