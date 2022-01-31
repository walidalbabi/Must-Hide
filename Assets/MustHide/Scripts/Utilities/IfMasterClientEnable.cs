using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class IfMasterClientEnable : MonoBehaviour
{


    [SerializeField] private GameObject StartBtn;
    [SerializeField] private GameObject PrivateBtn;
    [SerializeField] private GameObject PublicBtn;
    [SerializeField] private TextMeshProUGUI RoomCodeText;


    private void OnEnable()
    {
        RoomCodeText.text = "Code : " + PhotonNetwork.CurrentRoom.Name;
    }
    private void Update()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            StartBtn.SetActive(true);
            RoomCodeText.gameObject.SetActive(true);
            if (PhotonNetwork.CurrentRoom.Players.Count == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                StartBtn.GetComponent<Button>().interactable = true;
            }
            else
            {
                StartBtn.GetComponent<Button>().interactable = false;
            }

            if (PhotonNetwork.CurrentRoom.IsVisible)
            {
                PrivateBtn.SetActive(true);
                PublicBtn.SetActive(false);
            }
            else
            {
                PrivateBtn.SetActive(false);
                PublicBtn.SetActive(true);
            }
        }
        else
        {
            StartBtn.SetActive(false);
            PrivateBtn.SetActive(false);
            PublicBtn.SetActive(false);

            if (PhotonNetwork.CurrentRoom.IsVisible)
            {
                RoomCodeText.gameObject.SetActive(true);
            }
            else
            {
                RoomCodeText.gameObject.SetActive(false);
            }
        }
        
    }

    public void SetRoomPrivate()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public void SetRoomPublic()
    {
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }
}
