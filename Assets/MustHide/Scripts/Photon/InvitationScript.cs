using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InvitationScript : MonoBehaviour
{
    [SerializeField]
    private Text _text;

   

    public void SetInvitationInfo(string senderName)
    {
        _text.text = senderName + " Invited You!";
    }

    public void Accept()
    {
       
    }

    public void Reject()
    {

    }
}
