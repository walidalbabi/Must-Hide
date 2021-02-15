using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;
using VivoxUnity;

public class InGameManager : MonoBehaviour
{


    public enum State
    {
        ChooseCharacter,
        StartGame,
        EscapeTime,
        EndGame,
    }
    public State GameState;

    [SerializeField]
    private Text pingTxt;
    public static InGameManager instance;
    public int HuntersDead = 0;
    public int MonstersDead = 0;
   
    public Transform[] MonstersSpawnPoints;
    public Transform[] HuntersSpawnPoints;
    public Transform[] EscapeSpawnPoints;

    public int nextPlayerTeam;
    public int CurrentTeam;


    [SerializeField]
    private GameObject[] TasksAndEffects;

    private bool isPortalActive = true;

    public int WinnerTeam;

    [SerializeField]
    private Text MuteBtnText;
    [SerializeField]
    private Text DefeanBtnText;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity);
    }

    void Update()
    {
        pingTxt.text = PhotonNetwork.GetPing().ToString();
        if (MonstersDead >= 3 || HuntersDead >= 3)
        {
           GameState = State.EndGame;
        }

        switch (GameState)
        {
            case State.ChooseCharacter:
                MatchTimerManager.instance.isChoosePanel = true;
                break;
            case State.StartGame:
                MatchTimerManager.instance.isChoosePanel = false;
                if (!TasksAndEffects[0].activeInHierarchy)
                    TasksAndEffects[0].SetActive(true);
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
        if (MonstersDead >= 3)
        {
            //Hunters Win
            MatchTimerManager.instance.GameEndText.text = "Game End Hunters Win";

            if (PhotonNetwork.IsMasterClient)
                WinnerTeam = 1;
        }
        if (HuntersDead >= 3)
        {
            //Monsters Win
            MatchTimerManager.instance.GameEndText.text = "Game End Monsters Win";

            if (PhotonNetwork.IsMasterClient)
                WinnerTeam = 2;
        }
        SetWinnerTeam();
        MatchTimerManager.instance.timer = 0;
    }

    private void Escape_State()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (isPortalActive)
            {
                for (int i = 0; i < 2; i++)
                {
                    int index = Random.Range(0, 3);
                    if (EscapeSpawnPoints[index] != null)
                        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Portal"), EscapeSpawnPoints[index].transform.position, Quaternion.identity);
                    EscapeSpawnPoints[index] = null;
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

    public SoundAudioClip[] soundAudioClip;

    [System.Serializable]
    public class SoundAudioClip
    {
        public AudioManager.Sound sound;
        public AudioClip audioClip;
    }


   [PunRPC]
   private void RPC_WinnerTeam()
    {
#pragma warning disable CS1717 // Assignment made to same variable
        WinnerTeam = WinnerTeam;
#pragma warning restore CS1717 // Assignment made to same variable
    }

    public void SetWinnerTeam()
    {

        if (PhotonNetwork.IsMasterClient)
            GetComponent<PhotonView>().RPC("RPC_WinnerTeam", RpcTarget.Others);
    }
    #endregion  GameManager
}
