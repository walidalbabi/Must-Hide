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
    private GameObject ShopPanel;
    [SerializeField]
    private GameObject FriendListPanel;
    [SerializeField]
    private GameObject ProfilePanel;
    [SerializeField]
    private Image ProfileImg;
    [SerializeField]
    private Text[] ProfileInfo;
    [SerializeField]
    private Slider XPSlider;

    [SerializeField]
    private GameObject[] BtnLockMonsters; 
    [SerializeField]
    private Button[] InteractableBtnMonsters;
    [SerializeField]
    private GameObject[] BtnLockHunters; 
    [SerializeField]
    private Button[] InteractableBtnHunters;

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

    private void LeaveCurrentChannel()
    {
        VivoxManager.instance.LeaveChannel(true);
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

    public void SetProfileInfo()
    {
        StartCoroutine(LoadPlayerImg());
        ProfileInfo[0].text = Photon.Pun.PhotonNetwork.AuthValues.UserId;
        ProfileInfo[1].text = "Level : "+ PlayfabCloudSaving.instance._Level;
        ProfileInfo[2].text = "Nova : " + PlayfabCloudSaving.instance._Nova;
        ProfileInfo[3].text = "Eyes : " + PlayfabCloudSaving.instance._Eyes;
        ProfileInfo[4].text = "Total Wins : " + PlayfabCloudSaving.instance._TotalWins;
        ProfileInfo[5].text = "Total Loses : " + PlayfabCloudSaving.instance._TotalLoses; 
        ProfileInfo[6].text = "Total Kills : " + PlayfabCloudSaving.instance._TotalKills;
        ProfileInfo[7].text = PlayfabCloudSaving.instance._Xp + "/" + PlayfabCloudSaving.instance._MaxXp;

        XPSlider.maxValue = PlayfabCloudSaving.instance._MaxXp;
        XPSlider.value = PlayfabCloudSaving.instance._Xp;


    }
    IEnumerator LoadPlayerImg()
    {
        ProfileImg.color = Color.red;
        var playerImgUrl = PlayFabLogin.instance.PlayerInfo.AccountInfo.TitleInfo.AvatarUrl;
        WWW wwwLoader = new WWW(playerImgUrl);
        yield return wwwLoader;
        var imgSprite = Sprite.Create(wwwLoader.texture, new Rect(0, 0, 512, 512), Vector2.zero);
        ProfileImg.sprite = imgSprite;
        ProfileImg.color = Color.white;
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
            ShopPanel.SetActive(false);
        }

        if (state == "Play")
        {
            StartPanel.SetActive(false);
            PlayPanel.SetActive(true);
            CreateCustomMatchPanel.SetActive(false);
            JoinRoomPanel.SetActive(false);
            FriendListPanel.SetActive(false);
            ShopPanel.SetActive(false);
        }

    }

    public void ShowShopPanel()
    {
        ShopPanel.SetActive(true);
        StartPanel.SetActive(false);
        SetUpData();
    }

    public void CreateCustomMatch()
    {
        StartPanel.SetActive(false);
        CreateCustomMatchPanel.SetActive(true);
    }

    public void CloseProfilePanel()
    {
        ProfilePanel.SetActive(false);
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

    public void ShowPlayerProfile()
    {
        ProfilePanel.SetActive(true);
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

    public void SetUpData()
    {
        for (int i = 0; i < PlayfabCloudSaving.instance.MonstersCharacters.Length; i++)
        {
            BtnLockMonsters[i].SetActive(!PlayfabCloudSaving.instance.MonstersCharacters[i]);
            InteractableBtnMonsters[i].interactable = !PlayfabCloudSaving.instance.MonstersCharacters[i];
        } 
        for (int i = 0; i < PlayfabCloudSaving.instance.HuntersCharacters.Length; i++)
        {
            BtnLockHunters[i].SetActive(!PlayfabCloudSaving.instance.HuntersCharacters[i]);
            InteractableBtnHunters[i].interactable = !PlayfabCloudSaving.instance.HuntersCharacters[i];
        }
    }

    public void UnlockMonster(int index)
    {
        PlayfabCloudSaving.instance.MonstersCharacters[index] = true;
        PlayfabCloudSaving.instance.SetUserData(PlayfabCloudSaving.instance.CharatersStringToData(true), true);
        SetUpData();
    }

    public void UnlockHunters(int index)
    {
        PlayfabCloudSaving.instance.HuntersCharacters[index] = true;
        PlayfabCloudSaving.instance.SetUserData(PlayfabCloudSaving.instance.CharatersStringToData(false), false);
        SetUpData();
    }
    #endregion Playfab




}
