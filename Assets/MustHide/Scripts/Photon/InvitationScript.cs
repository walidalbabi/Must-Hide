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

        LoadingScript.instance.StartLoading("Joining"); 
        VivoxManager.instance.LeaveChannel(false);

        StartCoroutine(joinn());
        
    }

    IEnumerator joinn()
    {
        yield return new WaitForSeconds(0.5f);
        if (VivoxManager.instance.canJoin)
        {
            DelayedJoin();
            StopCoroutine(joinn());
        }
        else
            StartCoroutine(joinn());
    }


    public void Reject()
    {
        Destroy(gameObject);
    }

    private void DelayedJoin()
    {
      //  LoadingScript.instance.StopLoading();
        VivoxManager.instance.JoinChannel(ChannelLink, true, false, true, VivoxUnity.ChannelType.NonPositional);
        Destroy(gameObject);
    }
}
