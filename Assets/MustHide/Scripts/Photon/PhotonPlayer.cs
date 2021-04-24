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
    GameObject playerAvatar;
    public bool canCreatePlayer;
    [SerializeField]
    private string characterName;
    
    public bool isChoosPanel = true;
    public bool isPlayerDead;
    public string UserID;

    public GameObject selectedChar, beforeChar;
    public string Charc;
    public GameObject[] Monsters;
    public GameObject[] Hunters;
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
                PV.RPC("RPC_SetphotonPlayer", RpcTarget.AllBuffered);

        }

        Invoke("SetChooseCharVar", 2f);

        
    }

    

    // Update is called once per frame
    void Update()
    {

        if (!PV.IsMine)
            return;


        //Check Selected Character In Choose Character Panel
        if (InGameManager.instance.GameState == InGameManager.State.ChooseCharacter && PV.IsMine)
        {
            if (ChooseCharScript.instance.Name != "" || ChooseCharScript.instance.Name != "Recruiter")
            {
                if (myTeam == 1)
                    foreach (GameObject obj in Monsters)
                    {
                        if (obj.name == ChooseCharScript.instance.Name)
                        {
                            if (selectedChar != obj)
                                PV.RPC("RPC_CharVariables", RpcTarget.AllBuffered, obj.name);
                        }

                    }

                if (myTeam == 2)
                    foreach (GameObject obj in Hunters)
                    {
                        if (obj.name == ChooseCharScript.instance.Name)
                        {
                            if (selectedChar != obj)
                                PV.RPC("RPC_CharVariables", RpcTarget.AllBuffered, obj.name);
                        }

                    }
            }
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
            playerAvatar.GetComponent<Health>().SetPlayerTeam((byte)myTeam);
            playerAvatar.GetComponent<Health>().photonPlayer = this;
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
        VivoxManager.instance.JoinChannel(Photon.Pun.PhotonNetwork.CurrentRoom.Name.ToString()+myTeam.ToString(), true, false, true, VivoxUnity.ChannelType.NonPositional);
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
    [PunRPC]
    private void RPC_CharVariables(string name)
    {
        if (selectedChar != null)
            PV.RPC("RPC_ChooseChar", RpcTarget.AllBuffered, true);
        Charc = name;
        Invoke("SetSelectedChar", 0.3f);
        Invoke("SyncChoosePanelAfter", 0.45f);


    }
    [PunRPC]
    private void RPC_ChooseChar(bool activate)
    {
        if (selectedChar != null && selectedChar.name != "Recruiter")
            selectedChar.SetActive(activate);
    }

    private void SetSelectedChar()
    {
        if (myTeam == 1)
            foreach (GameObject obj in Monsters)
            {
                if (obj.name == Charc)
                {
                    if (obj.activeInHierarchy)
                        selectedChar = obj;
                }
            }

        if (myTeam == 2)
            foreach (GameObject obj in Hunters)
            {
                if (obj.name == Charc)
                {
                    if (obj.activeInHierarchy)
                        selectedChar = obj;
                }
            }

    }

    private void SyncChoosePanelAfter()
    {
        if (selectedChar != null && selectedChar.activeInHierarchy)
        {
            PV.RPC("RPC_ChooseChar", RpcTarget.AllBuffered, false);
        }
    }

    //Setting GameObjects After Scene Loaded
    private void SetChooseCharVar()
    {
        Monsters = new GameObject[ChooseCharScript.instance.Monsters.Length];
        Hunters = new GameObject[ChooseCharScript.instance.Hunters.Length];

        for (int i = 0; i < ChooseCharScript.instance.Monsters.Length; i++)
        {
            Monsters[i] = ChooseCharScript.instance.Monsters[i];
        }


        for (int i = 0; i < ChooseCharScript.instance.Hunters.Length; i++)
        {
            Hunters[i] = ChooseCharScript.instance.Hunters[i];
        }

        if (myTeam == 1)
            for (int i = 1; i < Monsters.Length; i++)
            {
                if (PlayfabCloudSaving.instance.MonstersCharacters[i - 1] == false)
                {
                    Monsters[i].GetComponent<Button>().interactable = false;
                }
                else
                {
                    Monsters[i].GetComponent<Button>().interactable = true;
                }
            }

        if (myTeam == 2)
            for (int i = 1; i < Hunters.Length; i++)
            {
                if (PlayfabCloudSaving.instance.HuntersCharacters[i - 1] == false)
                {
                    Hunters[i].GetComponent<Button>().interactable = false;
                }
                else
                {
                    Hunters[i].GetComponent<Button>().interactable = true;
                }
            }
    }
    #endregion ChoosePanel
}
