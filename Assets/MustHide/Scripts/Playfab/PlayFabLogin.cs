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
            PlayFabSettings.TitleId = "5A92F"; // Please change this value to your own titleId from PlayFab Game Manager
        }


        if (PlayerPrefs.HasKey("EMAIL") && PlayerPrefs.HasKey("PASSWORD"))
        {
            LoadingScript.instance.StartGameLoading("Logging in..");
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }

    }

    #region CallBacks
    private void OnLoginSuccess(LoginResult result)
    {
        isLoggedIn = true;
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        GetAccountInfo();
        PlayfabCloudSaving.instance.GetUserData(result.PlayFabId);
        MenuManager.instance.ShowHome();
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        isLoggedIn = true;
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        UpdateContactEmail();
        PlayfabCloudSaving.instance.GetUserData(result.PlayFabId);
        MenuManager.instance.ShowHome();
    }

    private void OnLoginFailure(PlayFabError error)
    {

        Debug.LogError(error.GenerateErrorReport());
        LoginFailedText.text = "Email address: is not valid Password: is not valid";
        ResetPassBtn.SetActive(true);
    }

    private void OnRegisterFailure(PlayFabError error)
    {

        Debug.LogError(error.GenerateErrorReport());
        RegisterFailedText.text = "Email address/Nickname already exists";
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
        NetworkManager.instance.ConnectToServer(result.AccountInfo.Username);
        NetworkManager.instance.CreateName(result.AccountInfo.Username);
        PlayfabCloudSaving.instance.GetStatistics();
        //VivoxManager.instance.Login(result.AccountInfo.Username, VivoxUnity.SubscriptionMode.Accept);
        PlayerIDText.text =  NetworkManager.instance.GetUserID();
    }

    private void OnUpdateAccountInfoSuccess(AddOrUpdateContactEmailResult result)
    {
        GetAccountInfo();

    }

    private void OnUpdateAccountInfoFaill(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        UpdateContactEmail();
    }

    private void OnGetAccountInfoFail(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        PlayerIDText.text = error.ToString();
    }

    private void OnAddFriendFailed(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnAddFriendSuccess(AddFriendResult result)
    {
        Debug.Log("Success Playfab Friend");
        NetworkManager.instance.FindFriend(friendName);
    }

    private void OnRemoveFriendFailed(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        Debug.Log("Success Remove Friend");
    }

    private void OnGetFriendListFail(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
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

    private void UpdateContactEmail()
    {
        LoadingScript.instance.StartGameLoading("Updating Contact Info..");
        var requestAddInfo = new AddOrUpdateContactEmailRequest { EmailAddress = userEmail };
        PlayFabClientAPI.AddOrUpdateContactEmail(requestAddInfo, OnUpdateAccountInfoSuccess, OnUpdateAccountInfoFaill);
        //UpdateAvatarRequest();
    }

    private void GetAccountInfo()
    {
        LoadingScript.instance.StartGameLoading("Getting Contact Info..");
        GetAccountInfoRequest request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFail);
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
           
        }

    }

    public void GetFriendList()
    {

        var request = new GetFriendsListRequest
        {
          
        };
        PlayFabClientAPI.GetFriendsList(request, OnGetFriendListSuccess, OnGetFriendListFail);
    }




    #endregion Functions

    public void DestroyObj()
    {
        Destroy(gameObject);
    }

}