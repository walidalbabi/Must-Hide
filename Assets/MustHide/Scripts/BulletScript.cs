using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BulletScript : MonoBehaviour
{

    public float BulletDamage = 5f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<PhotonView>().RPC("DestroyBullet", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void DestroyBullet()
    {

        Destroy(gameObject);
    }
}
