using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private byte _MaxPlayers = 10;
    [SerializeField]
    private string _Map;

    public const string MAP_PROP_KEY = "map";
    public const string GAME_MODE_PROP_KEY = "gm";


  
    [SerializeField]
    private string FriendName;



    [HideInInspector]
    public List<FriendScript> _listings = new List<FriendScript>();

    public string MyID;

    private bool isRank;

    // Instance 
    public static NetworkManager instance;


    private int m_LastPhotonACKTime = 0; private double m_LastPhotonACKTimeReceived = 0d;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);


        DontDestroyOnLoad(gameObject);
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
            if(PhotonNetwork.CurrentRoom.Players.Count == _MaxPlayers)
            {
               ChangeScene();
            }
        }

        //Checking For Connections

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ErrorScript.instance.StartErrorMsg("No Internet Connection, Please Check Your Internet Connection And Restart The Game", false, true, "photon");
        }

        if (PhotonNetwork.IsConnected)
        {
            if (m_LastPhotonACKTime != PhotonNetwork.NetworkingClient.LoadBalancingPeer.LastSendAckTime)
            {
                m_LastPhotonACKTime = PhotonNetwork.NetworkingClient.LoadBalancingPeer.LastSendAckTime;
                m_LastPhotonACKTimeReceived = PhotonNetwork.Time;
            }
            else if (0d != m_LastPhotonACKTimeReceived && PhotonNetwork.Time - m_LastPhotonACKTimeReceived > 15)
            {
                ErrorScript.instance.StartErrorMsg("You have Bad Connection , Please Check Your Connection than Reconnect", false, true, "photon");
                PhotonNetwork.Disconnect();
            }
        }
    }


    //CallBacks--------------------------------------------------
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master Server");
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        ErrorScript.instance.StopErrorMsg();
        if (!VivoxManager.instance.isVivoxLoggedIn)
            VivoxManager.instance.Login(PhotonNetwork.NickName, VivoxUnity.SubscriptionMode.Accept);

   

        MenuManager.instance.SetProfileInfo();

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        switch (cause)
        {
            case DisconnectCause.MaxCcuReached:
                ErrorScript.instance.StartErrorMsg("Servers Are Full Please Try Again", false, true, "photon");
                break;
            case DisconnectCause.ServerTimeout:
                ErrorScript.instance.StartErrorMsg("Server Timeout Please Try Again", false, true, "photon");
                break;
            case DisconnectCause.ClientTimeout:
                ErrorScript.instance.StartErrorMsg("Client Timeout Please Try Again", false, true, "photon");
                break;
        }
           
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

    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        base.OnFriendListUpdate(friendList);
        Debug.Log("OnFriendListUpdate got called");
        foreach (FriendInfo info in friendList)
        {
            Debug.Log(info.UserId + " Friend Already Exist");
            AddFriendListing(info);
        }
    }

    private void AddFriendListing(FriendInfo player)
    {
        int index = _listings.FindIndex(x => x.FriendInfo.UserId == player.UserId);

        if (index != -1)
        {
            _listings[index].SetRoomInfo(player);
        }
        else
        {
            var listing = Instantiate(MenuManager.instance._FriendListing, MenuManager.instance._Friendcontent);

            if (listing != null)
            {
                listing.SetRoomInfo(player);
                _listings.Add(listing);
            }
        }
    }

    public void RemoveFriendListing(FriendInfo otherPlayer)
    {
        int index = _listings.FindIndex(x => x.FriendInfo.UserId == otherPlayer.UserId);

        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    //Room Functions--------------------------------------------------

    public void ConnectToServer(string ID)
    {
        SetPlayerID(ID);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        LoadingScript.instance.StartGameLoading("Connecting To Game Server...");
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
        option.PlayerTtl = 20;
        Hashtable roomProperties = new Hashtable() {{ GAME_MODE_PROP_KEY, true } };

        string[] lobbyProperties = { GAME_MODE_PROP_KEY };


        option.CustomRoomPropertiesForLobby = lobbyProperties;
        option.CustomRoomProperties = roomProperties;
        if (expectedPlayers[0] != "")
            PhotonNetwork.CreateRoom("Rk" + RoomName.ToString(), option, TypedLobby.Default, expectedPlayers);
        else
            PhotonNetwork.CreateRoom("Rk" + RoomName.ToString(), option, TypedLobby.Default);
    }

   public  string[] expectedPlayers = new string[4];
    public void JoinRankMatch()
    {
      if(VivoxManager.instance._listings.Count >= 1)
        for (int i = 0; i < VivoxManager.instance._listings.Count; i++)
            {
                if(VivoxManager.instance._listings[i].Player.Account.Name != MyID)
                expectedPlayers[i] = VivoxManager.instance._listings[i].Player.Account.Name;
            }


        var expectedMaxPlayer = VivoxManager.instance._listings;
        string sqlLobbyFilter = "gm = 'true'";
        isRank = true;
        Hashtable roomProperties = new Hashtable() { { GAME_MODE_PROP_KEY, true } };
         if (expectedPlayers[0] != "")
            PhotonNetwork.JoinRandomRoom(roomProperties, (byte)expectedMaxPlayer.Count, MatchmakingMode.FillRoom, TypedLobby.Default, sqlLobbyFilter, expectedPlayers);
        else
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

        Hashtable roomProperties = new Hashtable() { { GAME_MODE_PROP_KEY, false }  };

        string[] lobbyProperties = { GAME_MODE_PROP_KEY};


        option.CustomRoomPropertiesForLobby = lobbyProperties;
        option.CustomRoomProperties = roomProperties;

        if (expectedPlayers[0] != "")
            PhotonNetwork.CreateRoom("Cl" + RoomName.ToString(), option, TypedLobby.Default, expectedPlayers);
        else
            PhotonNetwork.CreateRoom("Cl" + RoomName.ToString(), option, TypedLobby.Default);
    }

    public void JoinCasualMatch()
    {
        if (VivoxManager.instance._listings.Count >= 1)
            for (int i = 0; i < VivoxManager.instance._listings.Count; i++)
            {
                if (VivoxManager.instance._listings[i].Player.Account.Name != MyID)
                    expectedPlayers[i] = VivoxManager.instance._listings[i].Player.Account.Name;
            }

        var expectedMaxPlayer = VivoxManager.instance._listings;
        string sqlLobbyFilter = "gm = 'false'";

        isRank = false;
        Hashtable roomProperties = new Hashtable() { { GAME_MODE_PROP_KEY, false } };
        if (expectedPlayers[0] != "")
            PhotonNetwork.JoinRandomRoom(roomProperties, (byte)expectedMaxPlayer.Count, MatchmakingMode.FillRoom, TypedLobby.Default, sqlLobbyFilter, expectedPlayers);
        else
        {
            PhotonNetwork.JoinRandomRoom(roomProperties, 0);
            Debug.Log("Casusal");
        }

    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(roomName);
        }
 
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

    public void FindFriend(string userID)
    {
        PhotonNetwork.FindFriends(new string[] { userID });
        Debug.Log("Adding Friend... " + userID);
    }

    public string GetUserID()
    {
        return PhotonNetwork.AuthValues.UserId;
    }



    public void DestroyObj()
    {
        Destroy(gameObject);
    }

}
