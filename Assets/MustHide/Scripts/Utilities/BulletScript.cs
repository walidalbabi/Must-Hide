using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BulletScript : MonoBehaviour, IPooledObjects
{

    public float BulletDamage = 5f;
    public Health healthScript;


    private void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * 20f);
    }


    public void OnObjectsSpawn()
    {
        GetComponent<PhotonView>().RPC("RPC_SycnActive", RpcTarget.OthersBuffered);
    }

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

    [PunRPC]
    private void RPC_SycnActive()
    {

        gameObject.SetActive(true);
    }

}
