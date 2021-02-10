using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;
public class InGameManager : MonoBehaviour
{
    [SerializeField]
    private Text pingTxt;
    public static InGameManager instance;
    public int HuntersDead = 0;
    public int MonstersDead = 0;

    public bool GameIsOver;

   
    public Transform[] MonstersSpawnPoints;
    public Transform[] HuntersSpawnPoints;

    public int nextPlayerTeam;


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
        if (MonstersDead >= 5 || HuntersDead >= 5)
        {
            GameIsOver = true;
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


  //  [PunRPC]
    public void UpdateMonsterDead()
    {
        MonstersDead++;
    }

 //   [PunRPC]
    public void UpdateHuntersDead()
    {
        HuntersDead++;
    }
}
