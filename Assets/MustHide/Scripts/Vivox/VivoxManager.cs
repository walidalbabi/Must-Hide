using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
using System.ComponentModel;
using System;


public class  VivoxManager : MonoBehaviour
{

    public  VivoxCredentials vivox = new VivoxCredentials();

    public static VivoxManager instance;
    public string CurrentChannel;
    public string BeforeChannel;

    public bool disconnectedFromChannel, disconnectedFromAudio, disconnectedFromText, canJoin;

    public bool isVivoxLoggedIn;


    public List<PartyListingScript> _listings = new List<PartyListingScript>();

    [SerializeField]
    string msgSended;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        vivox.client = new Client();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        vivox.client.Uninitialize();
        vivox.client.Initialize();
    }

    private void OnApplicationQuit()
    {
        vivox.client.Uninitialize();
    }

    private void Update()
    {
        if(disconnectedFromChannel && disconnectedFromAudio)
        {
            canJoin = true;
        }
        else
        {
            canJoin = false;
        }

       
    }



    #region Binding Callbacks


    public void Bind_Login_Callback_Listeners(bool bind, ILoginSession loginSesh)
    {
        if (bind)
        {
            loginSesh.PropertyChanged += Login_Status;
        }
        else
        {
            loginSesh.PropertyChanged -= Login_Status;
        }
    }

    public void Bind_Channel_Callback_Listeners(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.PropertyChanged += On_Channel_Status_Changed;
        }
        else
        {
            channelSesh.PropertyChanged -= On_Channel_Status_Changed;
        }
    }

    public void Bind_User_Callbacks(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.Participants.AfterKeyAdded += On_Participant_Added;
            channelSesh.Participants.BeforeKeyRemoved += On_Participant_Removed;
            channelSesh.Participants.AfterValueUpdated += On_Participant_Updated;
        }
        else
        {
            channelSesh.Participants.AfterKeyAdded -= On_Participant_Added;
            channelSesh.Participants.BeforeKeyRemoved -= On_Participant_Removed;
            channelSesh.Participants.AfterValueUpdated -= On_Participant_Updated;
        }
    }

    public void Bind_Group_Message_Callbacks(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.MessageLog.AfterItemAdded += On_Message_Recieved;
        }
        else
        {
            channelSesh.MessageLog.AfterItemAdded -= On_Message_Recieved;
        }
    }

    public void Bind_Directed_Message_Callbacks(bool bind, ILoginSession loginSesh)
    {
        if (bind)
        {
            loginSesh.DirectedMessages.AfterItemAdded += On_Direct_Message_Recieved;
            loginSesh.FailedDirectedMessages.AfterItemAdded += On_Direct_Message_Failed;
        }
        else
        {
            loginSesh.DirectedMessages.AfterItemAdded -= On_Direct_Message_Recieved;
            loginSesh.FailedDirectedMessages.AfterItemAdded -= On_Direct_Message_Failed;
        }
    }

    #endregion
    

    #region Login Methods


    public void Login(string userName)
    {
        AccountId accountId = new AccountId(vivox.issuer, userName, vivox.domain);
        vivox.loginSession = vivox.client.GetLoginSession(accountId);

        Bind_Login_Callback_Listeners(true, vivox.loginSession);

        vivox.loginSession.BeginLogin(vivox.server, vivox.loginSession.GetLoginToken(vivox.tokenKey, vivox.timeSpan), ar =>
        {
            try
            {
                vivox.loginSession.EndLogin(ar);
            }
            catch (Exception e)
            {
                Bind_Login_Callback_Listeners(false, vivox.loginSession);
                Debug.Log(e.Message);
            }
            // run more code here 
        });
    }

    public void Login(string userName, SubscriptionMode subMode)
    {
        AccountId accountId = new AccountId(vivox.issuer, userName, vivox.domain);
        vivox.loginSession = vivox.client.GetLoginSession(accountId);
        
        Bind_Login_Callback_Listeners(true, vivox.loginSession);
        Bind_Directed_Message_Callbacks(true, vivox.loginSession);

        vivox.loginSession.BeginLogin(vivox.server, vivox.loginSession.GetLoginToken(vivox.tokenKey, vivox.timeSpan),subMode, null, null, null, ar =>
        {
            try
            {
                vivox.loginSession.EndLogin(ar);
                ErrorScript.instance.StopErrorMsg();
            }
            catch (Exception e)
            {
                Bind_Login_Callback_Listeners(false, vivox.loginSession);
                Bind_Directed_Message_Callbacks(false, vivox.loginSession);
                ErrorScript.instance.StartErrorMsg("Failed To Login To Vivox : " + e.Message, false , true, "vivox");
                LoadingScript.instance.StopGameLoading();
                Debug.Log(e.Message);
            }
            // run more code here 
        });
    }

    public void Logout()
    {
        vivox.loginSession.Logout();
        Bind_Login_Callback_Listeners(false , vivox.loginSession);
    }

    public void Login_Status(object sender, PropertyChangedEventArgs loginArgs)
    {
        var source = (ILoginSession)sender;

        if (loginArgs.PropertyName == "State")
        {
            switch (source.State)
            {
                case LoginState.LoggingIn:
                    Debug.Log("Logging In");
                    LoadingScript.instance.StartGameLoading("Connecting To Chat Services...");
                    break;

                case LoginState.LoggedIn:
                    Debug.Log($"Logged In {vivox.loginSession.LoginSessionId.Name}");
                    JoinChannel("channel" + Photon.Pun.PhotonNetwork.AuthValues.UserId, true, false, true, VivoxUnity.ChannelType.NonPositional);
                    isVivoxLoggedIn = true;
                    LoadingScript.instance.StopGameLoading();
                    break;
            }
        }

    }



    #endregion


    #region Join Channel Methods


    public void JoinChannel(string channelName, bool IsAudio, bool IsText, bool switchTransmission,ChannelType channelType)
    {
        
        ChannelId channelId = new ChannelId(vivox.issuer, channelName, vivox.domain, channelType);
        vivox.channelSession = vivox.loginSession.GetChannelSession(channelId);
        Bind_Channel_Callback_Listeners(true, vivox.channelSession);
        Bind_User_Callbacks(true, vivox.channelSession);
        Bind_Group_Message_Callbacks(true, vivox.channelSession);

        if (IsAudio)
        {
            vivox.channelSession.PropertyChanged += On_Audio_State_Changed;
        }
        if (IsText)
        {
            vivox.channelSession.PropertyChanged += On_Text_State_Changed;
        }


        vivox.channelSession.BeginConnect(IsAudio, IsText, switchTransmission, vivox.channelSession.GetConnectToken(vivox.tokenKey, vivox.timeSpan),ar => 
        {
            try
            {
                vivox.channelSession.EndConnect(ar);
            }
            catch(Exception e)
            {
                Bind_Channel_Callback_Listeners(false, vivox.channelSession);
                Bind_User_Callbacks(false, vivox.channelSession);
                Bind_Group_Message_Callbacks(false, vivox.channelSession);
                if (IsAudio)
                {
                    vivox.channelSession.PropertyChanged -= On_Audio_State_Changed;
                }
                if (IsText)
                {
                    vivox.channelSession.PropertyChanged -= On_Text_State_Changed;
                }
                Debug.Log(e.Message);
            }
        });
    }

    public void LeaveChannel(bool isForce)
    {
        vivox.channelSession.Disconnect();

        if (isForce)
            StartCoroutine(joinn());

    }


    IEnumerator joinn()
    {
        yield return new WaitForSeconds(0.5f);
        if (canJoin)
        {
            DelayedJoin();
            StopCoroutine(joinn());
        }
        else
            StartCoroutine(joinn());
    }

    private void DelayedJoin()
    {
        JoinChannel("channel" + Photon.Pun.PhotonNetwork.AuthValues.UserId, true, false, true, ChannelType.NonPositional);
        LoadingScript.instance.StopLoading();
    }

    public void On_Channel_Status_Changed(object sender, PropertyChangedEventArgs channelArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        if(channelArgs.PropertyName == "ChannelState")
        {
            switch (source.ChannelState)
            {
                case ConnectionState.Connecting:
                    Debug.Log($"{source.Channel.Name} Connecting");
                    //   LoadingScript.instance.StartLoading("Joining");
                    disconnectedFromChannel = false;
                    break;
                case ConnectionState.Connected:
                    Debug.Log($"{source.Channel.Name} Connected");
                    BeforeChannel = CurrentChannel;
                    LoadingScript.instance.StopLoading();
                    disconnectedFromChannel = false;        
                    CurrentChannel = source.Channel.Name;
                    break;
                case ConnectionState.Disconnecting:
                    Debug.Log($"{source.Channel.Name} disconnecting");
                    //  LoadingScript.instance.StartLoading("Leaving Party");
                    disconnectedFromChannel = false;
                    break;
                case ConnectionState.Disconnected:
                    Debug.Log($"{source.Channel.Name} disconnected");
                  //  LoadingScript.instance.StopLoading();
                    disconnectedFromChannel = true;
                  
                    //Events
                    Bind_Channel_Callback_Listeners(false, vivox.channelSession);
                    Bind_User_Callbacks(false, vivox.channelSession);
                    Bind_Group_Message_Callbacks(false, vivox.channelSession);         
                    break;
            }
        }
    }

    public void On_Audio_State_Changed(object sender, PropertyChangedEventArgs audioArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        if(audioArgs.PropertyName == "AudioState")
        {
            switch (source.AudioState)
            {
                case ConnectionState.Connecting:
                    Debug.Log($"Audio Channel Connecting");
                    disconnectedFromAudio = false;
                    break;
                case ConnectionState.Connected:
                    Debug.Log($"Audio Channel Connected");
                    disconnectedFromAudio = false;
                    break;
                case ConnectionState.Disconnecting:
                    Debug.Log($"Audio Channel Disconnecting");
                    disconnectedFromAudio = false;
                    break;
                case ConnectionState.Disconnected:
                    Debug.Log($"Audio Channel Disconnected");
                    disconnectedFromAudio = true;
                    vivox.channelSession.PropertyChanged -= On_Audio_State_Changed;
                    break;
            }
        }
    }  
    
    public void On_Text_State_Changed(object sender, PropertyChangedEventArgs textArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        if(textArgs.PropertyName == "TextState")
        {
            switch (source.TextState)
            {
                case ConnectionState.Connecting:
                    Debug.Log($"Text Channel Connecting");
                    disconnectedFromText = false;

                    break;
                case ConnectionState.Connected:
                    Debug.Log($"Text Channel Connected");

                    disconnectedFromText = false;
                    break;
                case ConnectionState.Disconnecting:
                    Debug.Log($"Text Channel Disconnecting");
                    disconnectedFromText = false;
                    break;
                case ConnectionState.Disconnected:
                    Debug.Log($"Text Channel Disconnected");

                    disconnectedFromText = true;
                    vivox.channelSession.PropertyChanged -= On_Text_State_Changed;
                    break;
            }
        }
    }

    #endregion


    #region User Callbacks


    public void On_Participant_Added(object sender, KeyEventArg<string> participantArgs)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        IParticipant user = source[participantArgs.Key];

        if (MenuManager.instance._Partycontent != null && MenuManager.instance._partyListing != null)
        {
            foreach (var participant in user.ParentChannelSession.Participants)
            {
                RemovePlayerListing(participant);
            }

            foreach (var participant in user.ParentChannelSession.Participants)
            {
                AddPlayerListing(participant);
            }

        }

        Debug.Log($"{user.Account.Name} has joined the channel");
    }   

    public void On_Participant_Removed(object sender, KeyEventArg<string> participantArgs)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        IParticipant user = source[participantArgs.Key];

        if (MenuManager.instance._Partycontent != null && MenuManager.instance._partyListing != null)
        {
            if (!user.IsSelf)
                RemovePlayerListing(user);
            else
            {
                foreach (var participant in user.ParentChannelSession.Participants)
                {
                    RemovePlayerListing(participant);
                }
            }
        }
 
        Debug.Log($"{user.Account.Name} has left the channel");

    }  

    public void On_Participant_Updated(object sender, ValueEventArg<string, IParticipant> participantArgs)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;
      
        IParticipant user = source[participantArgs.Key];

     
    }

    private void AddPlayerListing(IParticipant player)
    {
        int index = _listings.FindIndex(x => x.Player.Account.Name == player.Account.Name);

        if (index != -1)
        {
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            PartyListingScript listing = Instantiate(MenuManager.instance._partyListing, MenuManager.instance._Partycontent);

            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }
        }
    }

    public  void RemovePlayerListing(IParticipant otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player.Account.Name == otherPlayer.Account.Name);

        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    #endregion


    #region Message Methods

    public void Send_Group_Message(string message)
    {
        vivox.channelSession.BeginSendText(message, ar =>
        {
            try
            {
                vivox.channelSession.EndSendText(ar);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }   
    
    public void Send_Event_Message(string message, string stanzaNameSpace, string stanzaBody)
    {
        vivox.channelSession.BeginSendText(null, message, stanzaNameSpace, stanzaBody, ar =>
        {
            try
            {
                vivox.channelSession.EndSendText(ar);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }

    public void On_Message_Recieved(object sender, QueueItemAddedEventArgs<IChannelTextMessage> msgArgs)
    {
        var messenger = (VivoxUnity.IReadOnlyQueue<IChannelTextMessage>)sender;

        Debug.Log($"From {msgArgs.Value.Sender} : Message - {msgArgs.Value.Message}");

        Check_Message_Args(msgArgs.Value);
    }


    public void Check_Message_Args(IChannelTextMessage message)
    {
        if(message.ApplicationStanzaNamespace == "Test")
        {
            Debug.Log("This is a test");
            if(message.ApplicationStanzaBody == "blue")
            {
                Debug.Log("this player is blue");
            }
        }
        if(message.ApplicationStanzaBody == "Helloe Body")
        {
            Debug.Log("This a hidden message");
        }
    }


    #endregion


    #region Send Direct Messages

    public void Send_Direct_Message(string userToSend, string key ,string message)
    {
        var accountID = new AccountId(vivox.issuer, userToSend, vivox.domain);

        vivox.loginSession.BeginSendDirectedMessage(accountID,key + message, ar =>
        {
            try
            {
                vivox.loginSession.EndSendDirectedMessage(ar);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        });
    }

    public void On_Direct_Message_Recieved(object sender, QueueItemAddedEventArgs<IDirectedTextMessage> txtMsgArgs)
    {
        var msgSender = (IReadOnlyQueue<IDirectedTextMessage>)sender;

        while(msgSender.Count > 0)
        {
           var msg = msgSender.Dequeue().Message;
            Debug.Log(txtMsgArgs.Value.Message);

             msgSended = txtMsgArgs.Value.Message.Substring(0,3);

            if (msgSended == "inv")
            {
                var invitation = Instantiate(MenuManager.instance.invitation, MenuManager.instance._ChatContent);

                if (invitation != null)
                {
                    invitation.SetInvitationInfo(txtMsgArgs.Value.Sender.Name, txtMsgArgs.Value.Message.Substring(3));
                }
            }

            if (msgSended == "add")
            {
                var addFriend = Instantiate(MenuManager.instance.addFriend, MenuManager.instance._ChatContent);

                if (addFriend != null)
                {
                    addFriend.SetInvitationInfo(txtMsgArgs.Value.Sender.Name, txtMsgArgs.Value.Message.Substring(3));
                }
            }      
            
            if (msgSended == "acc")
            {
                PlayFabLogin.instance.AddFriends(txtMsgArgs.Value.Message.Substring(3));
            }

            if (msgSended == "rmv")
            {
                PlayFabLogin.instance.RemoveFriends(txtMsgArgs.Value.Message.Substring(3));
            }

            if (msgSended == "kck")
            {
                LeaveChannel(true);
            }

        }
    }

    public void On_Direct_Message_Failed(object sender, QueueItemAddedEventArgs<IFailedDirectedTextMessage> txtMsgArgs)
    {
        var msgSender = (IReadOnlyQueue<IFailedDirectedTextMessage>)sender;

        Debug.Log(txtMsgArgs.Value.Sender);
        vivox.failedMessages.Add(txtMsgArgs.Value);
    }


    #endregion


    public void DestroyObj()
    {
        Destroy(gameObject);
    }

}
