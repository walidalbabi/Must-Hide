using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class FriendList : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private Transform _content;
    [SerializeField]
    private FriendScript _friendListing;
    public List<GameObject> friendsObj = new List<GameObject>();

    public static FriendList instance;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        PlayFabLogin.instance.GetFriendList();
    }

    private void OnDisable()
    {
        for (int i = 0; i < friendsObj.Count; i++)
        {
            Destroy(friendsObj[i].gameObject);
            NetworkManager.instance._FriendList.RemoveAt(i);
            NetworkManager.instance._FriendsUserIDs.RemoveAt(i);
            friendsObj.RemoveAt(i);
            Debug.Log("Friend List Refreshed");
        }
    }

    public void UpdateFriendList()
    {

        for (int i = 0; i < friendsObj.Count; i++)
        {
            if (NetworkManager.instance._FriendList.Count >= 0)
            {
                Destroy(friendsObj[i].gameObject);
                NetworkManager.instance._FriendList.RemoveAt(i);
                NetworkManager.instance._FriendsUserIDs.RemoveAt(i);
                friendsObj.RemoveAt(i);
                Debug.Log("Friend List Refreshed");
            }
          
        }


        PlayFabLogin.instance.GetFriendList();


        if (NetworkManager.instance._FriendList.Count >= 0)
        {

            for (int i = 0; i < NetworkManager.instance._FriendList.Count; i++)
            {
                FriendScript listing = Instantiate(_friendListing, _content);
                friendsObj.Add(listing.gameObject);
                if (listing != null)
                {
                    listing.SetRoomInfo(NetworkManager.instance._FriendList[i]);
                }
            }

        }
    }
}
