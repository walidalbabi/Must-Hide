using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealBullet : MonoBehaviour
{
    [SerializeField]
    private float HealAmount = 30f;

    private float multiplier = 1f;

    private void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * 20f * multiplier);
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.GetComponent<Health>())
            collision.gameObject.GetComponent<Health>().HealForAmount(HealAmount);
        DestroyBullet();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponent<PhotonView>().IsMine)
            if (collision.gameObject.CompareTag("Shield"))
            {
                multiplier = -1f;
            }
    }



    private void DestroyBullet()
    {

        Destroy(gameObject);
    }
}
