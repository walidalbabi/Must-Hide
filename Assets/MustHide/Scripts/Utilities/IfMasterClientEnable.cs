using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IfMasterClientEnable : MonoBehaviour
{

    [SerializeField]
    private GameObject StartBtn;
    // Start is called before the first frame update
    void OnEnable()
    {
       
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            StartBtn.SetActive(true);
        else
            StartBtn.SetActive(false);
    }
}
