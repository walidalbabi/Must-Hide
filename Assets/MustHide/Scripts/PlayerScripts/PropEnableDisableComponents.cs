using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PropEnableDisableComponents : MonoBehaviour
{

    private PhotonView PV;

    [HideInInspector]
    public BlocksScript blockScript;
    [HideInInspector]
    public PhotonTransformView photonTransformView;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        blockScript = GetComponent<BlocksScript>();
    }

    public void OnPropSelected()
    {
        PV.RPC("syncSelected", RpcTarget.AllBuffered);
    }

    public void OnPropDeselected()
    {
        PV.RPC("syncDeselected", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void syncSelected()
    {
        blockScript.enabled = true;
    }

    [PunRPC]
    private void syncDeselected()
    {
        blockScript.enabled = false;
    }
}
