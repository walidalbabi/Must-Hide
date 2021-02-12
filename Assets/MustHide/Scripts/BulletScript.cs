using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BulletScript : MonoBehaviour
{

    public float BulletDamage = 5f;
    public Health healthScript;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<PhotonView>().RPC("DestroyBullet", RpcTarget.AllBuffered);


        if(GetComponent<PhotonView>().IsMine)
        if (collision.gameObject.CompareTag("Props"))
        {
            if (!collision.gameObject.GetComponent<BlocksScript>().isActiveAndEnabled)
            {
                healthScript.DoDamage(5);
            }
        }
    }


    //For Shooting Wrong Props
    public void SetBulletPlayerHealth(Health health)
    {
        healthScript = health;
    }

    [PunRPC]
    private void DestroyBullet()
    {

        Destroy(gameObject);
    }

}
