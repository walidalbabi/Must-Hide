using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public float BulletDamage = 5f;
    public Health healthScript;
    public bool canTakeDamage;

    private Rigidbody2D _rb;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (canTakeDamage)
        {
            _renderer.color = Color.white;
        }
        else _renderer.color = Color.green;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Props"))
        {
            if (!collision.gameObject.GetComponent<BlocksScript>().isActiveAndEnabled)
            {
                if (canTakeDamage)
                    healthScript.DoDamage(3.25f);
            }
        }

        if (collision.gameObject.CompareTag("Monster"))
        {
            healthScript.gameObject.GetComponent<ShootingScript>().SetEmunity();
        }
            DestroyBullet();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Shield"))
        {
            _rb.AddForce(transform.right * -800f * Time.deltaTime, ForceMode2D.Impulse);
        }
    }


    //For Shooting Wrong Props
    public void SetBulletPlayerHealth(Health health)
    {
        healthScript = health;
    }

    private void DestroyBullet()
    {
        gameObject.SetActive(false);
    }
}
