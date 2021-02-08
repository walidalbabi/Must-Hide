using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivoxUnity;

public class LobbyUI : MonoBehaviour
{
    public VivoxManager credentials;

    #region UI Variables
    // Updated UI variables not shown In my first video
    // The rest of the code is fairly the same
    public Text txt_UserName;
    [SerializeField] Text txt_ChannelName;
    public Text txt_Message_Prefab;
    [SerializeField] TMP_InputField tmp_Input_Username;
    [SerializeField] TMP_InputField tmp_Input_ChannelName;
    [SerializeField] TMP_InputField tmp_Input_SendMessages;
    public Image container;
    public TMP_Dropdown tmp_Dropdown_LoggedInUsers;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Btn_Join_Channel()
    {
        credentials.JoinChannel(tmp_Input_ChannelName.text, true, true, true, ChannelType.NonPositional);
    }

    public void Leave_Channel(IChannelSession channelToDiconnect, string channelName)
    {
        channelToDiconnect.Disconnect();
        credentials.vivox.loginSession.DeleteChannelSession(new ChannelId(credentials.vivox.issuer, channelName, credentials.vivox.domain));
    }

    public void Btn_Leave_Channel_Clicked()
    {
        Leave_Channel(credentials.vivox.channelSession, tmp_Input_ChannelName.text);
    }

    public void LoginUser()
    {
        credentials.Login(tmp_Input_Username.text, SubscriptionMode.Accept);
    }

    public void Logout()
    {
        credentials.vivox.loginSession.Logout();
        credentials.Bind_Login_Callback_Listeners(false, credentials.vivox.loginSession);
    }

    public void Send_Group_Message()
    {
        credentials.Send_Group_Message(tmp_Input_SendMessages.text);
    }  
    public void Send_Event_Message()
    {
        credentials.Send_Event_Message(tmp_Input_SendMessages.text, "Test", "blue");
    }

    public void Send_Direct_Message()
    {
        credentials.Send_Direct_Message(tmp_Dropdown_LoggedInUsers.Get_Selected(), tmp_Input_SendMessages.text);
    }
    

}
