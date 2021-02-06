using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IfMasterClientEnable : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        if (PhotonNetwork.IsMasterClient)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
