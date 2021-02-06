using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private bool isHunter;
  [HideInInspector]
    public bool cancameraFollow;

//    [HideInInspector]
    public float moveSpeed;

    Vector2 movement;
    Vector2 mousePos;

    //Components

    private Rigidbody2D rb;
    [HideInInspector]
    public PhotonView PV;
    [SerializeField]
    private SpriteRenderer SR;
    private Health health;

    //Camera
    public float dampTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        health = GetComponent<Health>();
        PV = GetComponent<PhotonView>();
        target = GetComponent<Transform>();
        if (SR == null)
            SR = GetComponent<SpriteRenderer>();

        if (PV.IsMine)
        {
            cancameraFollow = true;
        }
        else
            cancameraFollow = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

        GetInputs();

        if (cancameraFollow)
        {
            Vector3 point = cam.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = cam.transform.position + delta;
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, destination, ref velocity, dampTime);
        }
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        Move();

    }



    protected void GetInputs()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        SpriteManager();

        if (isHunter)
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
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
        //  transform.Translate(movement * moveSpeed * Time.deltaTime);
      

        if (isHunter)
        {
            Vector2 lookDir = mousePos - rb.position;

            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }


}
