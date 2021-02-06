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
}
