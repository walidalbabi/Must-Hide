using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaclonTrap : MonoBehaviour
{
    [SerializeField]
    private float Delay;

    private bool isStart;

    private void Start()
    {
        GetComponent<Photon.Pun.PhotonView>().TransferOwnership(-1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            if (!isStart)
            {
                isStart = true;
                StartCoroutine(StartTrap());
            }
        }

        if (collision.gameObject.CompareTag("SpiderWeb"))
        {
            Destroy(gameObject);
        }
    }


    IEnumerator StartTrap()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.tag = "FreezeTrap";
        GetComponent<AudioSource>().Play();
        GetComponent<CircleCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        GetComponent<CircleCollider2D>().enabled = true;
        yield return new WaitForSeconds(Delay);
        Destroy(gameObject);
    }
}
