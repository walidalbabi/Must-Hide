using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;

public class PartyListingScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _text;

    public IParticipant Player { get; private set; }
    [SerializeField]
    private string LeaderName;
    [SerializeField]
    string currentChannel;
    [SerializeField]
    private GameObject LeaderIcon;
    [SerializeField]
    private GameObject MemberIcon;
    [SerializeField]
    private Text MuteBtnText;

    //DropDown
    [SerializeField]
    private GameObject DropDown;
    [SerializeField]
    private GameObject ProfileBtn;
    [SerializeField]
    private GameObject MuteBtn;
    [SerializeField]
    private GameObject KickBtn;


    public bool isLeader;

    [HideInInspector]
    public GameObject LeaderGameObj;

    [SerializeField]
    private bool leaderAvailble;

    private void Start()
    {
       StartCoroutine(WaitLeaderToJoin());

        //if (!Player.IsSelf)
        //{
        //    MuteBtn.SetActive(true);
        //}
        //else
        //{
        //    MuteBtn.SetActive(false);
        //}

    }

    private void Update()
    {
      

        if (!Player.IsSelf)
            return;


        if(LeaderGameObj == null && !leaderAvailble)
        {
            for(int i =0; i< VivoxManager.instance._listings.Count; i++)
            {
                if (VivoxManager.instance._listings[i].isLeader)
                {
                    LeaderGameObj = VivoxManager.instance._listings[i].gameObject;
                    leaderAvailble = true;
                }
               
            }
        }
        else if(leaderAvailble && LeaderGameObj == null)
        {
            MenuManager.instance.LeaveParty();
        }


        if (isLeader)
        {
            MenuManager.instance.LockMatchBtns(false);
            MenuManager.instance.ShowLeavePartyBtn(false);
            LeaderIcon.SetActive(true);
            MemberIcon.SetActive(false);
        }
        else
        {
            MenuManager.instance.LockMatchBtns(true);
            MenuManager.instance.ShowLeavePartyBtn(true);
            LeaderIcon.SetActive(false);
            MemberIcon.SetActive(true);
        }
    }

    public void SetPlayerInfo(IParticipant player)
    {
         Player = player;
         currentChannel = "channel" + Player.Account.Name;
        _text.text = Player.Account.Name;

        Invoke("Delay", 1f);
    }

    private void Delay()
    {
        LeaderName = VivoxManager.instance.CurrentChannel.Remove(0 , 7);
        if (currentChannel == VivoxManager.instance.CurrentChannel)
        {
                isLeader = true;
        }
        else
        {
            isLeader = false;
        }
    }

    public void MuteMic()
    {
        if(Player.InAudio)
        if (Player.LocalMute == true)
        {
            MuteBtnText.text = "Mute";
            Player.LocalMute = false;
        }
        else
        {
            MuteBtnText.text = "Muted";
            Player.LocalMute = true;
        }
    }

    public void Kick()
    {
        VivoxManager.instance.Send_Direct_Message(Player.Account.Name, "kck", "You are Kicked");
    }

    public void ViewProfile()
    {
        MenuManager.instance.ShowPlayerProfile();
    }

    public void ViewDropDown()
    {
            if (DropDown.activeInHierarchy)
            {
                DropDown.SetActive(false);
            }
            else
            {
                DropDown.SetActive(true);
                Invoke("DisableDropDown", 3f);
            }

            if (Player.IsSelf)
            {
                ProfileBtn.SetActive(true);
                MuteBtn.SetActive(false);
            }
            else
            {
                ProfileBtn.SetActive(false);
                MuteBtn.SetActive(true);
            }

            if (isLeader)
            {
                KickBtn.SetActive(false);
            }
            else
            {
                KickBtn.SetActive(true);
            }
        
    }

    IEnumerator WaitLeaderToJoin()
    {
        yield return new WaitForSeconds(3f);

        if (!PhotonNetwork.InRoom)
        if (Player.IsSelf && !isLeader)
        {
                if (LeaderName != "")
                    PhotonNetwork.FindFriends(new string[] { LeaderName });
        }

   

        StartCoroutine(WaitLeaderToJoin());
    }


    public override void OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
    {
        if (friendList[0].IsInRoom)
        {
            StopAllCoroutines();
            NetworkManager.instance.JoinRoom(friendList[0].Room);
        }
    }

    private void DisableDropDown()
    {
        DropDown.SetActive(false);
    }

}
