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


    public static FriendList instance;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    public void UpdateFriendList()
    {

        for (int i = 0; i < NetworkManager.instance._FriendList.Count; i++)
        {
            FriendScript listing = Instantiate(_friendListing, _content);
            if (listing != null)
            {
                listing.SetRoomInfo(NetworkManager.instance._FriendList[i]);
            }
        }

    
    }
}
