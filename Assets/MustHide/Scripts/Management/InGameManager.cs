using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;
using VivoxUnity;
using Photon.Realtime;

public class InGameManager : MonoBehaviourPunCallbacks
{


    public enum State
    {
        ChooseCharacter,
        StartGame,
        EscapeTime,
        EndGame,
    }
    public State GameState;


    public static InGameManager instance;
    public int HuntersDead = 0;
    public int MonstersDead = 0;
    public int CollectedPortals = 0;

    private bool isPortalActive = true;

    public int WinnerTeam;
    public bool isPortalWin;

    int playerIndex;

    public int nextPlayerTeam;
    public int CurrentTeam;

    [SerializeField]
    private Text MuteBtnText;
    [SerializeField]
    private Text DefeanBtnText;
    [SerializeField]
    private Text pingTxt;

    public Transform[] MonstersSpawnPoints;
    public Transform[] HuntersSpawnPoints;
    public Transform[] EscapeSpawnPoints;




    [SerializeField]
    private GameObject[] Effects;




    public List<PhotonPlayer> photonPlayer = new List<PhotonPlayer>();
    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.Instantiate("PhotonNetworkPlayer", transform.position, Quaternion.identity);
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    void Update()
    {
        pingTxt.text = PhotonNetwork.GetPing().ToString();
        if (MonstersDead >= 5 || HuntersDead >= 5)
        {
           GameState = State.EndGame;
        }
        if(CollectedPortals >= 4)
        {
            SetPortal(true);
            GameState = State.EndGame;
        }

        switch (GameState)
        {
            case State.ChooseCharacter:
                PhotonNetwork.CurrentRoom.IsOpen = false;
                break;
            case State.StartGame:
             
                if (!Effects[0].activeInHierarchy)
                    Effects[0].SetActive(true);
                break;
            case State.EscapeTime:
                Escape_State();
                break;
            case State.EndGame:
                //If GameOver
                Invoke("SetGameOverPanel", 1.5f);
                break;
        }
    }







    #region HanldeGameState

    private void SetGameOverPanel()
    {
        MatchTimerManager.instance.EndPanel.SetActive(true);
        if (MonstersDead >= 5)
        {
            //Hunters Win
            MatchTimerManager.instance.GameEndText.text = "Game End Hunters Win";

            if (PhotonNetwork.IsMasterClient)
                WinnerTeam = 2;

        }else if (HuntersDead >= 5)
        {
            //Monsters Win
            MatchTimerManager.instance.GameEndText.text = "Game End Monsters Win";

            if (PhotonNetwork.IsMasterClient)
                WinnerTeam = 1;
        }
        else if (isPortalWin)
        {
            //Monsters Win
            MatchTimerManager.instance.GameEndText.text = "Game End Monsters Win";

            if (PhotonNetwork.IsMasterClient)
                WinnerTeam = 1;
        }
        else
        {
            //Hunters Win
            WinnerTeam = 2;
            MatchTimerManager.instance.GameEndText.text = "Game End Hunters Win";
        }


        SetWinnerTeam();
    }

    private void Escape_State()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (isPortalActive)
            {
                for (int i = 0; i < EscapeSpawnPoints.Length; i++)
                {
                        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Portal"), EscapeSpawnPoints[i].transform.position, Quaternion.identity);
                }
            }
               
        }

        if (isPortalActive)
        {
            MatchTimerManager.instance.EscapePanel.SetActive(true);
            isPortalActive = false;
        }

    }


    #endregion HanldeGameState


    #region GameManager



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

    public void MuteOtherUser()
    {
        IChannelSession currentChannelSession = VivoxManager.instance.vivox.loginSession.GetChannelSession(VivoxManager.instance.vivox.channelSession.Channel);
        
        var participants = currentChannelSession.Participants;

        foreach(var participant in currentChannelSession.Participants)
            if(!participant.IsSelf)
                if (participant.InAudio)
                {
                    if (participant.LocalMute == false)
                    {
                        participant.LocalMute = true;
                        DefeanBtnText.text = "Defeaned";
                    }
                    else
                    {
                        participant.LocalMute = false;
                        DefeanBtnText.text = "Defean";
                    }
                }
                else
                {
                    //Tell Player To Try Again
                    Debug.Log("Try Again");
                }
    }


    public void UpdatePlayerTeam()
    {
        if(nextPlayerTeam == 1)
        {
            nextPlayerTeam = 2;
        }
        else
        {
            nextPlayerTeam = 1;
        }
    }


    public void UpdateMonsterDead()
    {
        MonstersDead++;
    }

    public void UpdateHuntersDead()
    {
        HuntersDead++;
    }

    public void UpdateCollectedPortals()
    {
        CollectedPortals++;
    }

    public void SetPortal(bool portalBool)
    {
        GetComponent<Photon.Pun.PhotonView>().RPC("RPC_SetPortal", RpcTarget.AllBuffered, portalBool);
    }

    public void LeaveGame()
    {
        Photon.Pun.PhotonNetwork.LeaveRoom();
    }

    public SoundAudioClip[] soundAudioClip;

    [System.Serializable]
    public class SoundAudioClip
    {
        public AudioManager.Sound sound;
        public AudioClip audioClip;
    }

    public void SetWinnerTeam()
    {

        if (PhotonNetwork.IsMasterClient)
            GetComponent<PhotonView>().RPC("RPC_WinnerTeam", RpcTarget.Others);
    }

    //Rpc--------------------------------------------------------------

    [PunRPC]
    private void RPC_SetPortal(bool portalBool)
    {
        isPortalWin = portalBool;
    }

   [PunRPC]
   private void RPC_WinnerTeam()
    {
#pragma warning disable CS1717 // Assignment made to same variable
        WinnerTeam = WinnerTeam;
#pragma warning restore CS1717 // Assignment made to same variable
    }
    [PunRPC]
    private void RPC_UpdateHuntersDead()
    {
        UpdateHuntersDead();
    }

    [PunRPC]
    private void RPC_UpdateMonstersDead()
    {
        UpdateMonsterDead();
    }




    //Handle If Player Left Or Disconnect

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        //PhotonNetwork.LoadLevel(0);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        playerIndex = 0;
        foreach (var playerList in photonPlayer)
        {
            if(playerList.UserID == otherPlayer.UserId)
            {
                    if (playerList.myTeam == 0)
                    {
                        PhotonNetwork.LeaveRoom(true);
                        ErrorScript.instance.StartErrorMsg("Player Left The Game Before It Start", false, false, "");
                    }
                    else if (playerList.myTeam == 1 && !playerList.isPlayerDead)
                    {
                        GetComponent<PhotonView>().RPC("RPC_UpdateMonstersDead", RpcTarget.AllBuffered);
                    }
                    else if (playerList.myTeam == 2 && !playerList.isPlayerDead)
                    {
                        GetComponent<PhotonView>().RPC("RPC_UpdateHuntersDead", RpcTarget.AllBuffered);
                    }  
            }
            else
            {
                Debug.Log("Player Not in List");
            }
        }
       
        foreach (var playerList in photonPlayer)
        {
            if(playerList == null)
            {
                playerIndex++;
            }
        }

        if (playerIndex >= 10)
            PhotonNetwork.CurrentRoom.RemovedFromList = true;

            Debug.Log("Nick Name : "+otherPlayer.NickName+ "--- UserID: "+ otherPlayer.UserId);
        Debug.Log("Custom Properties : ");
    }

    #endregion  GameManager
}
