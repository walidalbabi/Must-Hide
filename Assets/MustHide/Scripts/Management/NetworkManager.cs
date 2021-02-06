using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private byte _MaxPlayers = 10;
    [SerializeField]
    private string _Map;
    private Text createRoomName;



    public const string MAP_PROP_KEY = "map";
    public const string GAME_MODE_PROP_KEY = "gm";

    public List<FriendInfo> _FriendList = new List<FriendInfo>();

    [SerializeField]
    private Text searchFriendField;
    [SerializeField]
    private string FriendName;

    public string MyID;

    private bool isRank;

    // Instance 
    public static NetworkManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)//if already exist
            gameObject.SetActive(false);
        else
        {
            //set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);
        }


    }


    private void Start()
    {
        SetPlayerName();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (PhotonNetwork.InRoom)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == _MaxPlayers)
            {
              //  ChangeScene();
            }
        }
    }


    //CallBacks--------------------------------------------------
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master Server");
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();

        LoadingScript.instance.StopLoading();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Succesful Create Match , GameMode : " + isRank);
        MenuManager.instance.ShowCurrentRoomPanel();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed To Create Match , GameMode : " + isRank);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Succesful Join Match , GameMode : " + isRank);
        MenuManager.instance.ShowCurrentRoomPanel();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed To Join Match , GameMode : " + isRank);
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed To Join Random Match , GameMode : " + isRank);

        if (isRank)
            CreateRank();
        else
            CreateCasual();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        //PhotonNetwork.FindFriends(new string[] { "guylard" , "Guylard", "1"});
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        base.OnFriendListUpdate(friendList);

        foreach (FriendInfo info in friendList)
        {
            if (info.IsOnline)
            {
                _FriendList.Add(info);
                FriendList.instance.UpdateFriendList();
            }
           
        }
    }

        //Room Functions--------------------------------------------------

    public void ConnectToServer(string ID)
    {
        SetPlayerID(ID);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        LoadingScript.instance.StartLoading("Connecting To Master...");
    }

        //Rank
        private void CreateRank()
    {
        if (!PhotonNetwork.IsConnected)
            return;

       int RoomName = Random.Range(1, 999999);

        RoomOptions option = new RoomOptions();
        option.MaxPlayers = _MaxPlayers;
        option.PublishUserId = true;
        Hashtable roomProperties = new Hashtable() {{ GAME_MODE_PROP_KEY, true } };

        string[] lobbyProperties = { GAME_MODE_PROP_KEY };


        option.CustomRoomPropertiesForLobby = lobbyProperties;
        option.CustomRoomProperties = roomProperties;

        PhotonNetwork.CreateRoom(RoomName.ToString(), option, TypedLobby.Default);
    }

    public void JoinRankMatch()
    {
        isRank = true;
        Hashtable roomProperties = new Hashtable() { { GAME_MODE_PROP_KEY, true } };
        PhotonNetwork.JoinRandomRoom(roomProperties, 0);
    }

    //Casual
    private void CreateCasual()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        int RoomName = Random.Range(1, 999999);

        RoomOptions option = new RoomOptions();
        option.MaxPlayers = _MaxPlayers;
        option.PublishUserId = true;

        Hashtable roomProperties = new Hashtable() { { GAME_MODE_PROP_KEY, false } };

        string[] lobbyProperties = { GAME_MODE_PROP_KEY };


        option.CustomRoomPropertiesForLobby = lobbyProperties;
        option.CustomRoomProperties = roomProperties;

        PhotonNetwork.CreateRoom(RoomName.ToString(), option, TypedLobby.Default);
    }

    public void JoinCasualMatch()
    {
        isRank = false;
        Hashtable roomProperties = new Hashtable() { { GAME_MODE_PROP_KEY, false } };
        PhotonNetwork.JoinRandomRoom(roomProperties, 0);
        Debug.Log("Casusal");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    //Photon Other Management---------------------------------------------------
    public void SetPlayerName()
    {
        if (PlayerPrefs.HasKey("PlayerName"))
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerName");
    }

    //Set Private userID
    public void SetPlayerID(string ID)
    {
        AuthenticationValues authValue = new AuthenticationValues(ID);
        PhotonNetwork.AuthValues = authValue;
        MyID = PhotonNetwork.AuthValues.UserId.ToString();
    }



    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
    }

     public void ChangeScene()
     {
        PhotonNetwork.LoadLevel(_Map);
        Debug.Log("Scene Loaded");
     }

    public void CreateName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
        SetPlayerName();
    }

    public void FindFriend()
    {
        if (searchFriendField.text != "")
            FriendName = searchFriendField.text;
        if (FriendName != "")
            PhotonNetwork.FindFriends(new string[] { FriendName });
    }

    public string GetUserID()
    {
        return PhotonNetwork.AuthValues.UserId;
    }

}
