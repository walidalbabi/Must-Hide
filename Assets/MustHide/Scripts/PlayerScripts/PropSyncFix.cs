using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PropSyncFix : MonoBehaviour
{

    private PhotonView PV;

    public bool isProp;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!PV.IsMine)
        {
            Transform collisionObjectRoot = collision.transform.root;
            if (collisionObjectRoot.CompareTag("Player"))
            {
                //Transfer PhotonView of Rigidbody to our local player
                PV.TransferOwnership(collision.gameObject.GetComponent<PhotonView>().Owner);
            }
        }
    }
}
