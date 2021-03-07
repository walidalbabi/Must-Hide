using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class LogsScript : MonoBehaviour
{

    PhotonView PV;

    [SerializeField]
    private Text txt;

    public string Color;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        transform.parent = InGameManager.instance.logsContent;
        txt.text = Color + PV.Controller.NickName + "</color>" + " " + "Died!";
    }

}
