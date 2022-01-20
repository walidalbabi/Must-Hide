using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class PortalScript : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] particle;
    [SerializeField]
    private GameObject portalLight;
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private float portalTimeToColl;

    private float portalCounter;

    bool canCount;
    bool isCount;
    bool isCollected;

    private PhotonView PV;

    private void Start()
    {
        PV =  GetComponent<PhotonView>();
        slider.maxValue = portalTimeToColl;


        PV.TransferOwnership(-1);
    }

    private void Update()
    {
        if (!isCollected)
        {
            slider.value = portalCounter;

            if (portalCounter <= 0)
            {
                slider.gameObject.SetActive(false);
            }
            else
            {
                slider.gameObject.SetActive(true);
            }

            if (canCount)
                if (isCount)
                {
                    if (portalCounter < portalTimeToColl)
                    {
                        portalCounter += Time.deltaTime;
                    }


                }
                else
                {
                    if (portalCounter > 0)
                    {
                        portalCounter -= Time.deltaTime;
                    }
                    else
                        canCount = false;
                }
        }
       
        if (PhotonNetwork.IsMasterClient)
            if (portalCounter >= portalTimeToColl && canCount)
            {
                canCount = false;
                PV.RPC("RPC_UpdateCollectedPortals", RpcTarget.AllBuffered);
            }

        if (isCollected)
        {
            slider.value = slider.maxValue;
            slider.fillRect.gameObject.GetComponent<Image>().color = Color.green;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            canCount = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            isCount = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            isCount = false;
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
     
        Invoke("DisableLightAfter" , 2f);
        GetComponent<AudioSource>().Pause();

       InGameManager.instance.UpdateCollectedPortals();
        isCollected = true;
    }

    private void DisableLightAfter()
    {
        Destroy(gameObject);
    }

}
