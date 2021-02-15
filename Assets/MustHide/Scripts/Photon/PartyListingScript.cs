using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;
using Photon.Pun;
using Photon.Realtime;

public class PartyListingScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text _text;

    public IParticipant Player { get; private set; }
    [SerializeField]
    private string LeaderName;
    [SerializeField]
    string myname;
    [SerializeField]
    private GameObject MuteBtn;
    [SerializeField]
    private Text MuteBtnText;

    public bool isLeader;

   // [HideInInspector]
    public GameObject LeaderGameObj;

    [SerializeField]
    private bool leaderAvailble;

    private void Start()
    {
       StartCoroutine(WaitLeaderToJoin());

        if (!Player.IsSelf)
        {
            MuteBtn.SetActive(true);
        }
        else
        {
            MuteBtn.SetActive(false);
        }
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
        }
        else
        {
            MenuManager.instance.LockMatchBtns(true);
            MenuManager.instance.ShowLeavePartyBtn(true);
        }
    }

    public void SetPlayerInfo(IParticipant player)
    {
         Player = player;
         myname = "channel" + Player.Account.Name;
    
        Invoke("Delay", 1f);
    }

    private void Delay()
    {
        LeaderName = VivoxManager.instance.CurrentChannel.Remove(0 , 7);
        if (myname == VivoxManager.instance.CurrentChannel)
        {
            _text.text = "(Leader) " + Player.Account.Name;
            Debug.Log("Leader");
            isLeader = true;
        }
        else
        {
            _text.text = Player.Account.Name;
            Debug.Log("member");
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


    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        if (friendList[0].IsInRoom)
        {
            StopAllCoroutines();
            NetworkManager.instance.JoinRoom(friendList[0].Room);
        }
    }

}
