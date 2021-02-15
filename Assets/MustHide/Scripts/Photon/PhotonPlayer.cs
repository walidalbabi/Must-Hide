using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotonPlayer : MonoBehaviour
{

    private PhotonView PV;
    public int myTeam;
    GameObject playerAvatar;
    public bool canCreatePlayer;
    [SerializeField]
    private string characterName;
    
    public bool isChoosPanel = true;

    public float GameTimer;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        //Getting Team
        if (PV.IsMine)
        {
                PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);     
        }



    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

        //Setting Timer by the Master Client
        GameTimer = MatchTimerManager.instance.timer;

        if (PhotonNetwork.IsMasterClient)
        {
             PV.RPC("RPC_UpdateMatchCounter", RpcTarget.Others, GameTimer);
        }



        SpawnCharacter();

        //Enable The Choose Character Panel
        if(isChoosPanel)
        if (!MatchTimerManager.instance.MonstersPanel.activeInHierarchy && !MatchTimerManager.instance.HuntersPanel.activeInHierarchy)
        {

                Invoke("SetChooseCharacterPanel", 2f);
            
        }

        if (MatchTimerManager.instance.createPlayer)
        {
            canCreatePlayer = true;
        }
    }

    private void SetChooseCharacterPanel()
    {
        InGameManager.instance.GameState = InGameManager.State.ChooseCharacter;
        MatchTimerManager.instance.StartPanel.SetActive(false);
        if (myTeam == 1)
        {
            MatchTimerManager.instance.MonstersPanel.SetActive(true);
            MatchTimerManager.instance.HuntersPanel.SetActive(false);
            isChoosPanel = false;
        }

        if (myTeam == 2)
        {
            MatchTimerManager.instance.MonstersPanel.SetActive(false);
            MatchTimerManager.instance.HuntersPanel.SetActive(true);
            isChoosPanel = false;
        }
    }
    private void SpawnCharacter()
    {
        if (myTeam != 0 && canCreatePlayer && playerAvatar == null)
        {
            if (myTeam == 1)
            {
                int spawnPicker = Random.Range(0, InGameManager.instance.MonstersSpawnPoints.Length);

                if (PV.IsMine)
                {
                    characterName = ChooseCharScript.instance.Name;
                    if (characterName == "")
                        characterName = "Recruiter";
                    playerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Monsters", characterName), InGameManager.instance.MonstersSpawnPoints[spawnPicker].position,
                           InGameManager.instance.MonstersSpawnPoints[spawnPicker].rotation, 0);
                }
            }
            else
            {
                int spawnPicker = Random.Range(0, InGameManager.instance.HuntersSpawnPoints.Length);

                if (PV.IsMine)
                {

                    characterName = ChooseCharScript.instance.Name;
                    if (characterName == "")
                        characterName = "Recruiter";
                    playerAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Hunters", characterName), InGameManager.instance.HuntersSpawnPoints[spawnPicker].position,
                         InGameManager.instance.HuntersSpawnPoints[spawnPicker].rotation, 0);
                }
            }
            InGameManager.instance.CurrentTeam = myTeam;
            playerAvatar.GetComponent<Health>().SetPlayerTeam(myTeam);
            //Join Team Channel
            VivoxManager.instance.LeaveChannel();
            StartCoroutine(joinn());
            InGameManager.instance.GameState = InGameManager.State.StartGame;

        }
       

    }

    //Channel Join Funtions
    IEnumerator joinn()
    {
        yield return new WaitForSeconds(0.5f);
        if (VivoxManager.instance.canJoin)
        {
            DelayedJoin();
            StopCoroutine(joinn());
        }
        else
            StartCoroutine(joinn());
    }

    private void DelayedJoin()
    {
        Debug.Log(Photon.Pun.PhotonNetwork.CurrentRoom.Name.ToString() + myTeam.ToString());
        VivoxManager.instance.JoinChannel(Photon.Pun.PhotonNetwork.CurrentRoom.Name.ToString()+myTeam.ToString(), true, false, true, VivoxUnity.ChannelType.NonPositional);
        Destroy(gameObject);
    }

    #region RPC


    [PunRPC]
    void RPC_GetTeam()
    {
        myTeam = InGameManager.instance.nextPlayerTeam;
        InGameManager.instance.UpdatePlayerTeam();
        PV.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, myTeam);
    }

    [PunRPC]
    void RPC_SentTeam(int wichTeam)
    {
        myTeam = wichTeam;
    }

    [PunRPC]
    void RPC_UpdateMatchCounter(float timer)
    {
        MatchTimerManager.instance.timer = timer;
    }

    #endregion RPC
}
