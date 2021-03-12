using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using VivoxUnity;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateCustomMatchPanel;
    [SerializeField]
    private GameObject JoinRoomPanel;
    [SerializeField]
    private GameObject CurrentRoomPanel;
    [SerializeField]
    private GameObject StartPanel;
    [SerializeField]
    private GameObject PlayPanel;
    [SerializeField]
    private GameObject FriendListPanel;

    [SerializeField]
    private GameObject[] MatchMackingButtons;

    [SerializeField]
    private GameObject LeavePartyBtn;

    [SerializeField]
    private GameObject LoginPanel;
    [SerializeField]
    private GameObject RegisterPanel;
    [SerializeField]
    private GameObject RecoveryPanel;


    [SerializeField]
    private Text MuteBtnText;


    public Transform _ChatContent;
    public InvitationScript invitation;
    public AddFriendScript addFriend;



    public static MenuManager instance;


    //Important Var's
    public Transform _Friendcontent;
    public FriendScript _FriendListing;
    public Text searchFriendField;

    public Transform _Partycontent;
    public PartyListingScript _partyListing;

    private string friendName;


    private void OnEnable()
    {
        if (PlayFabLogin.instance.isLoggedIn)
        {
            LoginPanel.SetActive(false);
            PlayPanel.SetActive(true);
        }

        if(VivoxManager.instance.CurrentChannel != "")
        {
            if (VivoxManager.instance.CurrentChannel != VivoxManager.instance.BeforeChannel)
            {
                LoadingScript.instance.StartLoading("Loading...");
                VivoxManager.instance._listings = new List<PartyListingScript>();
                LeaveCurrentChannel();
             //   Invoke("LeaveCurrentChannel", 1f);
         
            }
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)//if already exist
            gameObject.SetActive(false);
        else
        {
            //set the instance
            instance = this;
        }

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif

    }



    private void LeaveCurrentChannel()
    {
        VivoxManager.instance.LeaveChannel(true);
    }

    #region Photon

    public void JoinCasual()
    {
        NetworkManager.instance.JoinCasualMatch();
    }
    public void JoinRank()
    {
        NetworkManager.instance.JoinRankMatch();
    }

    public void StartMatch()
    {
        VivoxManager.instance._listings = new List<PartyListingScript>();
        NetworkManager.instance.ChangeScene();
    }

    #endregion Photon

    #region Vivox
    public void MuteMic()
    {
        if (VivoxManager.instance.vivox.client.AudioInputDevices.Muted == true)
        {
            MuteBtnText.text = "Mute";
            VivoxManager.instance.vivox.client.AudioInputDevices.Muted = false;
        }
        else
        {
            MuteBtnText.text = "Muted";
            VivoxManager.instance.vivox.client.AudioInputDevices.Muted = true;
        }
    }

    public void Defean()
    {

    }

    public void LockMatchBtns(bool isLock)
    {
        if (isLock)
        {
            foreach (GameObject btn in MatchMackingButtons)
                btn.SetActive(false);
        }
        else
            foreach (GameObject btn in MatchMackingButtons)
                btn.SetActive(true);


    }

    public void LeaveParty()
    {


        VivoxManager.instance.LeaveChannel(false);
        LoadingScript.instance.StartLoading("Leaving Party");

        //  Invoke("JoinChannelAfterLeave", 3f);
        StartCoroutine(joinn());
    }


    IEnumerator joinn()
    {
        yield return new WaitForSeconds(0.5f);
        if (VivoxManager.instance.canJoin)
        {
            JoinChannelAfterLeave();
            StopCoroutine(joinn());
        }
        else
            StartCoroutine(joinn());
    }

    private void JoinChannelAfterLeave()
    {
        //  LoadingScript.instance.StopLoading();

        VivoxManager.instance.BeforeChannel = "channel" + Photon.Pun.PhotonNetwork.AuthValues.UserId;
        VivoxManager.instance.JoinChannel("channel" + Photon.Pun.PhotonNetwork.AuthValues.UserId, true, false, true, ChannelType.NonPositional);
    }

    public void ShowLeavePartyBtn(bool show)
    {
        if (show)
            LeavePartyBtn.SetActive(true);
        else
            LeavePartyBtn.SetActive(false);
    }


    public void SendAddFriendRequest()
    {
        if(friendName != "")
        {
            VivoxManager.instance.Send_Direct_Message(friendName, "add", Photon.Pun.PhotonNetwork.AuthValues.UserId);
            Debug.Log("Invite Sended");
        }
        
    }

    #endregion Vivox

    #region UIManager

    public void GetFriendName(string friendNameIn)
    {
        InputField input = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.
       gameObject.GetComponent<InputField>();
        friendNameIn = input.text;
        friendName = friendNameIn;
    }
    public void ShowHome()
    {
        StartPanel.SetActive(true);
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
    }


    public void Play()
    {
        PlayPanel.SetActive(true);
        StartPanel.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Back(string state)
    {
        if (state == "Menu")
        {
            StartPanel.SetActive(true);
            PlayPanel.SetActive(false);
            CreateCustomMatchPanel.SetActive(false);
            JoinRoomPanel.SetActive(false);
        }

        if (state == "Play")
        {
            StartPanel.SetActive(false);
            PlayPanel.SetActive(true);
            CreateCustomMatchPanel.SetActive(false);
            JoinRoomPanel.SetActive(false);
            FriendListPanel.SetActive(false);
        }

    }

    public void CreateCustomMatch()
    {
        StartPanel.SetActive(false);
        CreateCustomMatchPanel.SetActive(true);
    }


    public void Public()
    {
        StartPanel.SetActive(false);
        JoinRoomPanel.SetActive(true);
    }

    public void ShowFriendsPanel()
    {
        PlayPanel.SetActive(false);
        FriendListPanel.SetActive(true);
    }

    public void ShowCurrentRoomPanel()
    {
        CreateCustomMatchPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        CurrentRoomPanel.SetActive(true);
        PlayPanel.SetActive(false);
    }

    public void ExitCurrentRoomPanel()
    {
        StartPanel.SetActive(true);
        CreateCustomMatchPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        CurrentRoomPanel.SetActive(false);
        NetworkManager.instance.LeaveRoom();
    }
    #endregion UIManager

    #region Playfab
    public void GoToLogin()
    {
        LoginPanel.SetActive(true);
        RegisterPanel.SetActive(false);
    }

    public void GoToRegister()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }

    public void GoToForgetPassword()
    {
        LoginPanel.SetActive(false);
        RecoveryPanel.SetActive(true);
    }

    #endregion Playfab




}
