using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
public class PlayerListingScript : MonoBehaviour
{

    [SerializeField]
    private Text _text;

    private Image _image;

    public Player Player { get; private set; }


    private void Start()
    {
        _image = GetComponent<Image>();
        _image.color = new Color32(142, 142, 142, 25);
    }

    public void SetPlayerInfo(Player player)
    {
        Player = player;
     
        _text.text = player.NickName;

    }

    private void Update()
    {
        if (Player.GetPhotonTeam() == null)
        {
            _image.color = new Color32(142, 142, 142, 25);
            return;
        }

        if (Player.GetPhotonTeam().Name == "Hunters")
        {
            _image.color = new Color32(0, 0, 255, 25);
        }
        else if (Player.GetPhotonTeam().Name == "Monsters")
        {
            _image.color = new Color32(195, 100, 12, 25);
        }
        else
        {
            _image.color = new Color32(142, 142, 142, 25);
        }


    }

}
