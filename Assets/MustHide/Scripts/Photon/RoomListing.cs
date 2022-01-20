using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListing : MonoBehaviourPunCallbacks
{


    [SerializeField]
    private Transform _content;
    [SerializeField]
    private RoomScript _roomListing;


    private List<RoomScript> _listings = new List<RoomScript>();

    public override void OnJoinedRoom()
    {
        _content.DestroyChildren();
        _listings.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            //Removed from rooms List
            if (info.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);

                if(index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }

            }
            //Added to Room List
            else
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);

                if(index == -1)
                {
                    RoomScript listing = Instantiate(_roomListing, _content);

                    if (listing != null)
                    {
                        listing.SetRoomInfo(info);
                        _listings.Add(listing);
                    }
                }
                else
                {
                    //Modify Listing here
                    //_listings[index].do..
                }   
                    
            }
           
        }
    }


}
