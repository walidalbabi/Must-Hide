using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class PhotonPlayer : MonoBehaviour
{
    [HideInInspector]
    public PhotonView PV;
    public int myTeam;
    GameObject playerCharacter;
    public bool canCreatePlayer;
    [SerializeField]
    private string characterName;
    
    public bool isChoosPanel = true;
    public bool isPlayerDead;
    public string UserID;

    //Choose Character Section
    public string choosenCharacterName;
    private CharacterSelectBtn _currentSelectedChar;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }


    void Start()
    {
 
        if (PV.IsMine)
        {
            //Getting Team
            PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
            //Adding Photon Player to list
            PV.RPC("RPC_SetphotonPlayer", RpcTarget.AllBuffered);

            ChooseCharScript.instance.SetCurrentPlayerComponent(this);

        }    
    }

    

    // Update is called once per frame
    void Update()
    {

        if (!PV.IsMine)
            return;


        //Check Selected Character In Choose Character Panel
        //if (InGameManager.instance.GameState == InGameManager.State.ChooseCharacter && PV.IsMine)
        //{
        //    if (ChooseCharScript.instance.Name != "" || ChooseCharScript.instance.Name != "Recruiter")
        //    {
        //        if (myTeam == 1)
        //            foreach (GameObject obj in Monsters)
        //            {
        //                if (obj.name == ChooseCharScript.instance.Name)
        //                {
        //                    if (selectedChar != obj)
        //                        PV.RPC("RPC_CharVariables", RpcTarget.AllBuffered, obj.name);
        //                }

        //            }

        //        if (myTeam == 2)
        //            foreach (GameObject obj in Hunters)
        //            {
        //                if (obj.name == ChooseCharScript.instance.Name)
        //                {
        //                    if (selectedChar != obj)
        //                        PV.RPC("RPC_CharVariables", RpcTarget.AllBuffered, obj.name);
        //                }

        //            }
        //    }
        //}


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
        MatchTimerManager.instance.ActivatePanel(false, MatchTimerManager.instance.StartPanel, MatchTimerManager.instance.exitGameCanvas, 0.5f);
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
        if (myTeam != 0 && canCreatePlayer && playerCharacter == null)
        {
            if (myTeam == 1)
            {
                int spawnPicker = Random.Range(0, InGameManager.instance.MonstersSpawnPoints.Length);

                if (PV.IsMine)
                {
                    characterName = choosenCharacterName;
                    if (characterName == "")
                        characterName = "Recruiter";
                    playerCharacter = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Monsters", characterName), InGameManager.instance.MonstersSpawnPoints[spawnPicker].position,
                           InGameManager.instance.MonstersSpawnPoints[spawnPicker].rotation, 0);
                }
            }
            else
            {
                int spawnPicker = Random.Range(0, InGameManager.instance.HuntersSpawnPoints.Length);

                if (PV.IsMine)
                {

                    characterName = choosenCharacterName;
                    if (characterName == "")
                        characterName = "Recruiter";
                    playerCharacter = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Hunters", characterName), InGameManager.instance.HuntersSpawnPoints[spawnPicker].position,
                         InGameManager.instance.HuntersSpawnPoints[spawnPicker].rotation, 0);
                }
            }
            InGameManager.instance.CurrentTeam = myTeam;
            playerCharacter.GetComponent<Health>().SetPlayerTeam((byte)myTeam);
            playerCharacter.GetComponent<Health>().photonPlayer = this;
            //Join Team Channel
            VivoxManager.instance.LeaveChannel(false);
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
        VivoxManager.instance.JoinChannel(Photon.Pun.PhotonNetwork.CurrentRoom.Name.Substring(0,5)+myTeam.ToString(), true, false, true, VivoxUnity.ChannelType.NonPositional);
    }

    #region RPC

    public void SetIsDead(bool isDead)
    {
        PV.RPC("RPC_SetIsDead", RpcTarget.AllBuffered, isDead);
    }

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
    void RPC_SetphotonPlayer()
    {
        UserID = PV.Owner.UserId;
        InGameManager.instance.photonPlayer.Add(this);
    }

    [PunRPC]
    void RPC_SetIsDead(bool deadbool)
    {
        isPlayerDead = deadbool;
    }

    

    #endregion RPC

    #region ChoosePanel
   
    public void SetChosenCharacterName(string name)
    {
        choosenCharacterName = name;
        PV.RPC("RPC_SetChosenCharacterName", RpcTarget.OthersBuffered, choosenCharacterName);

        if(PV.IsMine)
        if (_currentSelectedChar != null && _currentSelectedChar != ChooseCharScript.instance.CharacterNameToCharacterBtnComponent(choosenCharacterName))
            _currentSelectedChar.SetState(CharacterSeletctBtnState.Diselect);

        SetCharacterSelectedUIState();
    
    }

    private void ConfigureCurrentSelectedCharacter()
    {
        if (!PV.IsMine) return;
        _currentSelectedChar = ChooseCharScript.instance.CharacterNameToCharacterBtnComponent(choosenCharacterName);
        _currentSelectedChar.SetState(CharacterSeletctBtnState.IsSelected);
    }

    [PunRPC]
    private void RPC_SetChosenCharacterName(string name)
    {
        choosenCharacterName = name;
        SetCharacterSelectedUIState();
    }

    private void SetCharacterSelectedUIState()
    {
        ChooseCharScript.instance.UpdateState();
        ConfigureCurrentSelectedCharacter();
    }

    #endregion ChoosePanel
}
