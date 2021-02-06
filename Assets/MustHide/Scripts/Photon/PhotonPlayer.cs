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
        SpawnCharacter();
        if(MatchTimerManager.instance.Team == 0)
            MatchTimerManager.instance.Team = myTeam;


        if (MatchTimerManager.instance.createPlayer)
        {
            canCreatePlayer = true;
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
