using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletTracker : MonoBehaviour
{
    [SerializeField]
    private float delay;

    private float multiplier = 1f;
    private bool isHit;

    [SerializeField]
    private GameObject[] lights;

    private SpriteRenderer SR;

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        Invoke("DestroyBullet", delay);
    }

    private void Update()
    {
        if (!isHit)
            transform.Translate(Vector2.right * Time.deltaTime * 20f * multiplier);
        else
            transform.position = transform.parent.position;

    }



    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Hunter"))
        {
            foreach(GameObject L in lights)
            {
                L.SetActive(false);
            }
            SR.sprite = null;
            GetComponent<BoxCollider2D>().enabled = false;
            isHit = true;
            gameObject.layer = 8;
            transform.parent = collision.gameObject.transform;
        }
        else
        {
            DestroyBullet();
        }

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
