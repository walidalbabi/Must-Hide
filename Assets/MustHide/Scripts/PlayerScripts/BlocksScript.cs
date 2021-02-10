using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BlocksScript : MonoBehaviour
{

    public float moveSpeed;

    Vector2 movement;
    Vector2 mousePos;

    //Components

    private Rigidbody2D rb;
    [HideInInspector]
    public PhotonView PV;


    private SpriteRenderer SR;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PV = GetComponent<PhotonView>();
        if (SR == null)
            SR = GetComponent<SpriteRenderer>();

        PV.SetControllerInternal(-1);

    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

        GetInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }



    protected void GetInputs()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        SpriteManager();

    }

    protected void SpriteManager()
    {
        if (movement.x > 0)
        {
            SR.flipX = false;
        }
        else
        {
            if (movement.x < 0)
            {
                SR.flipX = true;
            }

        }
    }

    protected void Move()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
                //Transform Back
        }
    }


}
