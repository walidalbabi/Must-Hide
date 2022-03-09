using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class CheckTeamInRoom : MonoBehaviourPun
{

    [SerializeField] private Button _hunterBtn, _monsterBtn;

    // Update is called once per frame
    void Update()
    {
        if (PhotonTeamsManager.Instance.GetTeamMembersCount(1) >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            _monsterBtn.interactable = false;
        else _monsterBtn.interactable = true;


        if (PhotonTeamsManager.Instance.GetTeamMembersCount(2) >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            _hunterBtn.interactable = false;
        else _hunterBtn.interactable = true;
    }
}
