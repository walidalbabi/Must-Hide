using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BlocksScript : MonoBehaviour
{

    public PropsController propController;
    private PhotonView PV;





    //// Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    //// Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && propController != null)
        {
            //Transform Back
            propController.BackToTransformation();
        }
    }

    public void SetPropController(PropsController propControll)
    {

        propController = propControll;
       PV.RPC("RPC_SetPropController", RpcTarget.AllBuffered);
    }


    [PunRPC]
    public void RPC_SetPropController()
    {
#pragma warning disable CS1717 // Assignment made to same variable
        propController = propController;
#pragma warning restore CS1717 // Assignment made to same variable
    }
}
