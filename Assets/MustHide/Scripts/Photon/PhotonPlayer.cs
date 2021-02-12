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
    private bool canCreatePlayer;
    [SerializeField]
    private string characterName;

    private bool isChoosPanel = true;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {

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


        SpawnCharacter();


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


        }
       

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


}
