using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InvitationScript : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    public string ChannelLink;

    private void Start()
    {
        Destroy(gameObject, 30f);
    }
    public void SetInvitationInfo(string senderName, string channel)
    {
        ChannelLink = channel;
        _text.text = senderName + " Invited You!";
    }

    public void Accept()
    {
   
        VivoxManager.instance.JoinChannel(ChannelLink, true, false, true, VivoxUnity.ChannelType.NonPositional);
        Destroy(gameObject);
    }

    public void Reject()
    {
        Destroy(gameObject);
    }
}
