using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletTracker : MonoBehaviourPun
{
    [SerializeField]
    private float delay;

    private float multiplier = 1f;
    private bool isHit;

    [SerializeField]
    private GameObject[] lights;

    [SerializeField]private Transform _target;

    private SpriteRenderer SR;

    private void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        Invoke("DestroyBullet", delay);
    }

    private void Update()
    {
        if (!isHit)
        {
            if (_target == null)
                transform.Translate(Vector2.right * Time.deltaTime * 7f * multiplier);
            else
            {
                transform.Translate(Vector2.right * Time.deltaTime * 7f * multiplier);
                Vector3 dir = _target.position - transform.position;
                //get the angle from current direction facing to desired target
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //set the angle into a quaternion + sprite offset depending on initial sprite facing direction
                Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                //Roatate current game object to face the target using a slerp function which adds some smoothing to the move
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 3f * Time.deltaTime);
            }
        }
        else
            transform.position = transform.parent.position;


    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
            if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("Hunter"))
            {
                SetTracker(collision);
            }
            else
            {
                DestroyBullet();
            }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Monster"))
        {
            _target = collision.transform;
            GetComponent<CircleCollider2D>().enabled = false;
        }

    }


    private void SetTracker(Collision2D collision)
    {
        foreach (GameObject L in lights)
        {
            L.SetActive(false);
        }
        SR.sprite = null;
        GetComponent<BoxCollider2D>().enabled = false;
        isHit = true;
        gameObject.layer = 8;
        transform.parent = collision.gameObject.transform;
        collision.gameObject.GetComponent<Health>().SetCanUseAbility(false);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
