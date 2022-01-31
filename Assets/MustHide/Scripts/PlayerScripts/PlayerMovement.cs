using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
  [HideInInspector]
    public bool cancameraFollow;

   [HideInInspector]
    public float moveSpeed;

    public bool isMoving;

    [HideInInspector]
    public bool canMove;

    public Vector2 movement;

    //Components

    private Rigidbody2D rb;
    [HideInInspector]
    public PhotonView PV;
    public SpriteRenderer SR;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private GameObject MobileUI;
    [SerializeField]
    private GameObject PlayerUI;
    //Camera
    public float dampTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    public Transform target;

    [SerializeField]
    private GameObject miniMap;
    [SerializeField]
    private GameObject miniCam;

    //Traps
    [HideInInspector]
    public bool isSlowingDown;

    private Health _health;


    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
        cam = Camera.main;
        PV = GetComponent<PhotonView>();
        target = GetComponent<Transform>();
        if (SR == null)
            SR = GetComponent<SpriteRenderer>();

        if (!PV.IsMine)
            PlayerUI.SetActive(false);

        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            MobileUI.SetActive(false);
        }
        else
        {
            if (PV.IsMine)
            {
                MobileUI.SetActive(true);
            }
            else MobileUI.SetActive(false);

        }

        if (PV.IsMine)
        {
            miniMap.SetActive(true);
            miniCam.SetActive(true);
            cancameraFollow = true;
        }
        else
        {
            miniMap.SetActive(false);
            miniCam.SetActive(false);
            cancameraFollow = false;
        }

        GetComponent<AudioManager>().Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

        if (movement.x != 0 || movement.y != 0)
            isMoving = true;
        else
            isMoving = false;

        GetInputs();

        if (SystemInfo.deviceType == DeviceType.Desktop && _health.IsMonster)
            SetMouseState();
    }

    private void SetMouseState()
    {
        //HideCursor
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (Cursor.visible) Cursor.visible = false;
        }
        //EnableCursor
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Cursor.visible) Cursor.visible = true;
        }
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        Move();


        if (cancameraFollow)
        {
            Vector3 point = cam.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = cam.transform.position + delta;
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, destination, ref velocity, dampTime);
        }
    }



    protected void GetInputs()
    {
        if (canMove)
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");
            }
            else if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                movement.x = joystick.Horizontal;
                movement.y = joystick.Vertical;
            }
        }
        else
        {
            movement.x = 0f;
            movement.y = 0f;
        }
     

        animator.SetFloat("Horizontal", Mathf.Round(movement.x));
        animator.SetFloat("Vertical", Mathf.Round(movement.y));
        animator.SetFloat("speed", movement.sqrMagnitude);  
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


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("SpiderWeb"))
        {
            if (!GetComponent<PropsController>())
                canMove = false;
        }


        if (collision.gameObject.CompareTag("FreezeTrap"))
        {
            if (GetComponent<PropsController>())
                canMove = false;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("SpiderWeb"))
        {
            if (!GetComponent<PropsController>())
                canMove = true;
        }

        if (collision.gameObject.CompareTag("FreezeTrap"))
        {
            if (GetComponent<PropsController>())
                canMove = true;
        }
    }


}
