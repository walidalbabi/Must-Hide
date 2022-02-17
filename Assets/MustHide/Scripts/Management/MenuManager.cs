using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private GameObject SettingsPanel;
    [SerializeField]
    private GameObject FriendListPanel;
    [SerializeField]
    private GameObject ProfilePanel;
    [SerializeField]
    private GameObject PublicRooms;
    [SerializeField]
    private GameObject HowToPlayPanel;
    [SerializeField]
    private GameObject FirstLoginPanel;
    [SerializeField]
    private Image ProfileImg;
    [SerializeField]
    private TextMeshProUGUI[] ProfileInfo;
    [SerializeField]
    private Slider XPSlider;


    [SerializeField]
    private Button[] InteractableBtnMonsters;
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
    private GameObject muteBtn, unMuteBtn;

    [SerializeField]
    private GameObject HunterSection, MonsterSection, EyesSection;
    [SerializeField]
    private GameObject LogoutPanel;

    [SerializeField]
    private InputField JoinPrivateInputField;

    private CanvasGroup HunterCanvas, MonsterCanvas, EyesCanvas;
    private CanvasGroup LogoutPanelCanvas;
    private CanvasGroup FirstLoginPanelCanvas;

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

    public Slider[] audiSlider;

    private int _roomMaxPlayersNumb = 10;
    private Button _currentSelectedPlayerNumbButton;


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

    private void Start()
    {
        HunterCanvas = HunterSection.GetComponent<CanvasGroup>();
        MonsterCanvas = MonsterSection.GetComponent<CanvasGroup>();
        EyesCanvas = EyesSection.GetComponent<CanvasGroup>();
        FirstLoginPanelCanvas = FirstLoginPanel.GetComponent<CanvasGroup>();
        LogoutPanelCanvas = LogoutPanel.GetComponent<CanvasGroup>();

        Cursor.visible = true;


    }

    #region Photon

    public void JoinCasual()
    {
        NetworkManager.instance.JoinCasualMatch();
        LoadingScript.instance.StartLoading("Searching For a Game...");
    }
    public void JoinRank()
    {
        NetworkManager.instance.JoinRankMatch();
    }

    public void JoinPrivate()
    {
        NetworkManager.instance.JoinRoom(JoinPrivateInputField.text);
    }

    public void CreateCustomMatch()
    {
        NetworkManager.instance.CreateCustomMatch(_roomMaxPlayersNumb);
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
            VivoxManager.instance.vivox.client.AudioInputDevices.Muted = false;
            muteBtn.SetActive(true);
            unMuteBtn.SetActive(false);
        }
        else
        {
            VivoxManager.instance.vivox.client.AudioInputDevices.Muted = true;
            muteBtn.SetActive(false);
            unMuteBtn.SetActive(true);
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
        LoadingScript.instance.StartLoading("Leaving Party...");

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
       //StartCoroutine(LoadPlayerImg());
        ProfileInfo[0].text = Photon.Pun.PhotonNetwork.AuthValues.UserId;
        ProfileInfo[1].text = "Level : " + PlayfabCloudSaving.instance._Level.ToString();
        ProfileInfo[2].text = PlayfabCloudSaving.instance._Nova.ToString();
        ProfileInfo[3].text = PlayfabCloudSaving.instance._Eyes.ToString();
        ProfileInfo[4].text = PlayfabCloudSaving.instance._TotalWins.ToString();
        ProfileInfo[5].text = PlayfabCloudSaving.instance._TotalLoses.ToString(); 
        ProfileInfo[6].text = PlayfabCloudSaving.instance._TotalKills.ToString();
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
        PublicRooms.SetActive(false);
        CreateCustomMatchPanel.SetActive(false);
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
            SettingsPanel.SetActive(false);
            HowToPlayPanel.SetActive(false);
        }

        if (state == "Play")
        {
            StartPanel.SetActive(false);
            PlayPanel.SetActive(true);
            CreateCustomMatchPanel.SetActive(false);
            JoinRoomPanel.SetActive(false);
            FriendListPanel.SetActive(false);
            ShopPanel.SetActive(false);
            SettingsPanel.SetActive(false);
        }

    }

    public void ShowShopPanel()
    {
        ShopPanel.SetActive(true);
        StartPanel.SetActive(false);
        SetUpData();
    } 
    
    public void ShowHowToPlayPanel()
    {
        HowToPlayPanel.SetActive(true);
        StartPanel.SetActive(false);
    }

    public void ShowPublicRoomsPanel()
    {
        PublicRooms.SetActive(true);
        PlayPanel.SetActive(false);
    }

    public void ShowCreateCustomMatch()
    {
        PlayPanel.SetActive(false);
        CreateCustomMatchPanel.SetActive(true);
    }

    public void CloseProfilePanel()
    {
        ProfilePanel.SetActive(false);
    }

    public void Public()
    {
        PlayPanel.SetActive(false);
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
        CreateCustomMatchPanel.SetActive(false);
        JoinRoomPanel.SetActive(false);
        CurrentRoomPanel.SetActive(false);
        LoadingScript.instance.StartLoading("Loading Menu...");
        NetworkManager.instance.LeaveRoom();
    }

    public void ShowPlayerProfile()
    {
        ProfilePanel.SetActive(true);
    }

    public void ShowSettings()
    {
        SettingsPanel.SetActive(true);
        StartPanel.SetActive(false);
    }

    public void ShowFirstTimeLogin()
    {
        if (FirstLoginPanel.activeInHierarchy)
            FirstLoginPanel.SetActive(false);
        else
        {
            FirstLoginPanel.SetActive(true);
            FirstLoginPanelCanvas.alpha = 0;
            FirstLoginPanelCanvas.LeanAlpha(1f, 0.3f);
        }
    }

    public void GetPlayerRoomNumber()
    {
        if (_currentSelectedPlayerNumbButton != null) _currentSelectedPlayerNumbButton.interactable = true;

        _currentSelectedPlayerNumbButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        _currentSelectedPlayerNumbButton.interactable = false;

        _roomMaxPlayersNumb = int.Parse(_currentSelectedPlayerNumbButton.gameObject.name);
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

    public void Logout()
    {
        if (LogoutPanel.activeInHierarchy)
            LogoutPanel.SetActive(false);
        else
        {
            LogoutPanel.SetActive(true);
            LogoutPanel.transform.localScale = Vector3.zero;
            LogoutPanel.transform.LeanScale(new Vector3(1f,1f,1f), 0.3f);
            LogoutPanelCanvas.alpha = 0;
            LogoutPanelCanvas.LeanAlpha(1f, 0.3f);
        }

    }

    public void LogoutAndRestart()
    {
        PlayFabLogin.instance.Logout();
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
            InteractableBtnMonsters[i].interactable = !PlayfabCloudSaving.instance.MonstersCharacters[i];
        } 
        for (int i = 0; i < PlayfabCloudSaving.instance.HuntersCharacters.Length; i++)
        {
            InteractableBtnHunters[i].interactable = !PlayfabCloudSaving.instance.HuntersCharacters[i];
        }
    }

    public void HunterSectionInShop()
    {
        MonsterSection.SetActive(false);
        EyesSection.SetActive(false);
        HunterSection.SetActive(true);
        HunterCanvas.alpha = 0;
        HunterCanvas.LeanAlpha(1f, 0.2f);
    }

    public void MonsterSectionInShop()
    {
        MonsterSection.SetActive(true);
        EyesSection.SetActive(false);
        HunterSection.SetActive(false);
        MonsterCanvas.alpha = 0;
        MonsterCanvas.LeanAlpha(1f, 0.2f);
    }
    public void EyesSectionInShop()
    {
        MonsterSection.SetActive(false);
        EyesSection.SetActive(true);
        HunterSection.SetActive(false);
        EyesCanvas.alpha = 0;
        EyesCanvas.LeanAlpha(1f, 0.2f);
    }

    public void UnlockMonster(int index)
    {
        PlayfabCloudSaving.instance.MonstersCharacters[index] = true;
        PlayfabCloudSaving.instance.SetUserCharactersData(PlayfabCloudSaving.instance.CharatersStringToData(true), true, false, 0);
        SetUpData();
    }

    public void UnlockHunters(int index)
    {
        PlayfabCloudSaving.instance.HuntersCharacters[index] = true;
        PlayfabCloudSaving.instance.SetUserCharactersData(PlayfabCloudSaving.instance.CharatersStringToData(false), false, false, 0);
        SetUpData();
    }

    public void UnlockMonsterNova(int index, int price)
    {
        PlayfabCloudSaving.instance.MonstersCharacters[index] = true;
        PlayfabCloudSaving.instance.SetUserCharactersData(PlayfabCloudSaving.instance.CharatersStringToData(true), true, true, price);
        SetUpData();
    }

    public void UnlockHuntersNova(int index, int price)
    {
        PlayfabCloudSaving.instance.HuntersCharacters[index] = true;
        PlayfabCloudSaving.instance.SetUserCharactersData(PlayfabCloudSaving.instance.CharatersStringToData(false), false, true, price);
        SetUpData();
    }

    public void UnlockMonsterEyes(int index, int price)
    {
        PlayfabCloudSaving.instance.MonstersCharacters[index] = true;
        PlayfabCloudSaving.instance.SetUserCharactersData(PlayfabCloudSaving.instance.CharatersStringToData(true), true, false, price);
        SetUpData();
    }

    public void UnlockHuntersEyes(int index, int price)
    {
        PlayfabCloudSaving.instance.HuntersCharacters[index] = true;
        PlayfabCloudSaving.instance.SetUserCharactersData(PlayfabCloudSaving.instance.CharatersStringToData(false), false, false, price);
        SetUpData();
    }
    #endregion Playfab

}
