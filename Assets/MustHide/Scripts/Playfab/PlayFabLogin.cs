using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;
using UnityEngine.UI;


public class PlayFabLogin : MonoBehaviour
{

    public static PlayFabLogin instance;

    public bool isLoggedIn;

    [SerializeField]
    private string PlayerAvatarUrl;

    [SerializeField]
    private string userEmail;

    [SerializeField]
    private string userPassword;

    [SerializeField]
    private string userName;

    [SerializeField]
    private string friendName;

    [SerializeField]
    private GameObject ResetPassBtn;

    [SerializeField]
    private Text RegisterFailedText;
    [SerializeField]
    private Text LoginFailedText;
    [SerializeField]
    private Text RecoveryText;


    [SerializeField]
    private Text PlayerIDText;

    [HideInInspector]
    public GetAccountInfoResult PlayerInfo;

    private GetTitleDataResult _titleDataResult;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);

        #if UNITY_EDITOR
                Debug.unityLogger.logEnabled = true;
        #else
                Debug.unityLogger.logEnabled = false;
        #endif

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "76014"; // Please change this value to your own titleId from PlayFab Game Manager
        }


        if (PlayerPrefs.HasKey("EMAIL") && PlayerPrefs.HasKey("PASSWORD"))
        {
            LoadingScript.instance.StartGameLoading("Logging in..");
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            if (userEmail != "" && userPassword != "")
            {
                var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
                PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
            }
            else
            {
                LoadingScript.instance.StopGameLoading();
                MenuManager.instance.GoToLogin();
            }

        }

    }

    #region CallBacks
    private void OnLoginSuccess(LoginResult result)
    {
        isLoggedIn = true;
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        GetAccountInfo();
      
        MenuManager.instance.ShowHome();
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        isLoggedIn = true;
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        UpdateContactEmail();
      //  PlayfabCloudSaving.instance.GetUserData(result.PlayFabId);
        MenuManager.instance.ShowHome();
    }

    private void OnLoginFailure(PlayFabError error)
    {

        Debug.LogError(error.GenerateErrorReport());
        LoginFailedText.text = error.GenerateErrorReport();
        ResetPassBtn.SetActive(true);
        LoadingScript.instance.StopGameLoading();
        MenuManager.instance.GoToLogin();
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        RegisterFailedText.text = error.GenerateErrorReport();
        LoadingScript.instance.StopGameLoading();
        MenuManager.instance.GoToRegister();
    }

    private void FailUpdateCallBack(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnRecoveryEmailSuccess(SendAccountRecoveryEmailResult result)
    {
        RecoveryText.text = "Recovery Email Sended!!";
    }

    private void OnRecoveryEmailFailed(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        RecoveryText.text = "Invalid email Address";
    }

    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        PlayerInfo = result;
        GetTitleData();
    }

    private void OnUpdateAccountInfoSuccess(AddOrUpdateContactEmailResult result)
    {
        UpdatePlayerTitle();
    }

    private void OnGetTitleDataSuccess(GetTitleDataResult result)
    {
        _titleDataResult = result;

        if (!RegionManager.instance.CheckIfRegionSelected())
        {
            RegionManager.instance.ShowReigionPanel();
            return;
        }
        InstantiatePhotonAndPlayfabStatistics(PlayerPrefs.GetString("HasRegion"));
    }

    public void InstantiatePhotonAndPlayfabStatistics(string regionName)
    {
        NetworkManager.instance.ConnectToServer(regionName, PlayerInfo.AccountInfo.Username);
        NetworkManager.instance.CreateName(PlayerInfo.AccountInfo.Username);
        PlayerIDText.text = NetworkManager.instance.GetUserID();
        PlayfabCloudSaving.instance.GetStatistics();
    }

    private void OnUpdateAccountInfoFaill(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        UpdateContactEmail();
    }

    private void OnUpdatePlayerTitleFailed(PlayFabError obj)
    {
        UpdatePlayerTitle();
    }

    private void OnUpdatePlayerTitleSuccess(UpdateUserTitleDisplayNameResult obj)
    {
        GetAccountInfo();
    }

    private void OnGetAccountInfoFail(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        ErrorScript.instance.StartErrorMsg(error.GenerateErrorReport(), false , true,  false,"vivox");
    }

    private void OnAddFriendFailed(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        ErrorScript.instance.StartErrorMsg(error.GenerateErrorReport(), false, false, true,"");
    }

    private void OnAddFriendSuccess(AddFriendResult result)
    {
        Debug.Log("Success Playfab Friend");
        NetworkManager.instance.FindFriend(friendName);
    }

    private void OnRemoveFriendFailed(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        ErrorScript.instance.StartErrorMsg(error.GenerateErrorReport(), false, false, true,"");
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        Debug.Log("Success Remove Friend");
    }

    private void OnGetFriendListFail(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        ErrorScript.instance.StartErrorMsg(error.GenerateErrorReport(), false, false, true, "");
    }

    private void OnGetFriendListSuccess(GetFriendsListResult result)
    {
        Debug.Log("Success friend list loaded");
        for(int i = 0; i < result.Friends.Count; i++)
        {
            Debug.Log(result.Friends[i].Username);
            NetworkManager.instance.FindFriend(result.Friends[i].Username);
        }

    }

    private void OnGetTitleDataFail(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        ErrorScript.instance.StartErrorMsg(error.GenerateErrorReport(), false, true, false, "photon");
    }

    #endregion CallBacks

    #region Authentication
    public void GetUserEmail(string emaiIn)
    {
        InputField input = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.
            gameObject.GetComponent<InputField>();
        emaiIn = input.text;
        userEmail = emaiIn;
    }

    public void GetUserPassword(string passwordIn)
    {
        InputField input = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.
       gameObject.GetComponent<InputField>();
        passwordIn = input.text;
        userPassword = passwordIn;
    }

    public void GetUserName(string userNameIn)
    {
        InputField input = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.
       gameObject.GetComponent<InputField>();
        userNameIn = input.text;
        userName = userNameIn;
    }

    public void Logout()
    {
        PlayerPrefs.SetString("EMAIL", "");
        PlayerPrefs.SetString("PASSWORD", "");
        Application.Quit();
    }

    private void UpdateContactEmail()
    {
        LoadingScript.instance.StartGameLoading("Updating Contact Info..");
        var requestAddInfo = new AddOrUpdateContactEmailRequest { EmailAddress = userEmail };
        PlayFabClientAPI.AddOrUpdateContactEmail(requestAddInfo, OnUpdateAccountInfoSuccess, OnUpdateAccountInfoFaill);
        //UpdateAvatarRequest();
    }

    private void UpdatePlayerTitle()
    {
        LoadingScript.instance.StartGameLoading("Updating Player Title..");
        var requestAddInfo = new UpdateUserTitleDisplayNameRequest { DisplayName = userName };
        PlayFabClientAPI.UpdateUserTitleDisplayName(requestAddInfo, OnUpdatePlayerTitleSuccess, OnUpdatePlayerTitleFailed);
    }


    private void GetAccountInfo()
    {
        LoadingScript.instance.StartGameLoading("Getting Contact Info..");
        GetAccountInfoRequest request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFail);
    }

    private void GetTitleData()
    {
        LoadingScript.instance.StartGameLoading("Getting Data..");
        var request = new GetTitleDataRequest();
        PlayFabClientAPI.GetTitleData(request, OnGetTitleDataSuccess, OnGetTitleDataFail);
    }

    public void OnClickLogin()
    {
       // LoadingScript.instance.StartGameLoading("Logging in..");
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public void OnClickRegister()
    {
      //  LoadingScript.instance.StartGameLoading("Registering..");
        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = userName };
        
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);   
    }

    public void SendCustomAccountRecoveryEmail()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = userEmail,
            TitleId = PlayFabSettings.TitleId,
            EmailTemplateId = "8B1F4B01EE07C73D"
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoveryEmailSuccess, OnRecoveryEmailFailed);
    }

    public void UpdateAvatarRequest()
    {
        var requestUpdateAvatar = new UpdateAvatarUrlRequest { ImageUrl = PlayerAvatarUrl };
        PlayFabClientAPI.UpdateAvatarUrl(requestUpdateAvatar, null, null);
        MenuManager.instance.SetProfileInfo();
    }

    public void SetPlayerAvatarUrl(string Url)
    {
        PlayerAvatarUrl = Url;
        UpdateAvatarRequest();
    }
    #endregion Authentication

    #region Functions

    public void AddFriends(string friendName)
    {
        if (friendName != "")
        {
            var request = new AddFriendRequest
            {
                FriendUsername = friendName
            };

            PlayFabClientAPI.AddFriend(request, OnAddFriendSuccess, OnAddFriendFailed);

            GetFriendList();
        }
      
    }

    public void RemoveFriends(string FriendName)
    {
        if (FriendName != "")
        {
            var request = new RemoveFriendRequest
            {
                FriendPlayFabId = FriendName
            };

            PlayFabClientAPI.RemoveFriend(request, OnRemoveFriendSuccess, OnRemoveFriendFailed);

            GetFriendList();

        }

    }

    public void GetFriendList()
    {

        var request = new GetFriendsListRequest
        {
          
        };
        PlayFabClientAPI.GetFriendsList(request, OnGetFriendListSuccess, OnGetFriendListFail);
    }


    public GetTitleDataResult GetTitleDataVariable()
    {
        return _titleDataResult;
    }

    #endregion Functions

    public void DestroyObj()
    {
        Destroy(gameObject);
    }

}