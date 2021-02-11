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
    [HideInInspector]
    public PropSyncFix propSyncFix;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        blockScript = GetComponent<BlocksScript>();
       // photonTransformView = GetComponent<PhotonTransformView>();
        //propSyncFix = GetComponent<PropSyncFix>();
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
      //  photonTransformView.enabled = false;
       // propSyncFix.enabled = false;
    }

    [PunRPC]
    private void syncDeselected()
    {
        blockScript.enabled = false;
       // photonTransformView.enabled = true;
       // propSyncFix.enabled = true;
    }
}
