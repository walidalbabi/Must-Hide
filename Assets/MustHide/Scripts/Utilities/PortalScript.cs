using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PortalScript : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] particle;
    [SerializeField]
    private GameObject portalLight;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            GetComponent<PhotonView>().RPC("RPC_UpdateCollectedPortals", RpcTarget.AllBuffered);
        }
    }



    [PunRPC]
    private void RPC_UpdateCollectedPortals()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        foreach(var part in particle)
        {
            part.Stop();
        }
        portalLight.SetActive(false);
        GetComponent<AudioSource>().Pause();
        InGameManager.instance.UpdateCollectedPortals();
    }

}
