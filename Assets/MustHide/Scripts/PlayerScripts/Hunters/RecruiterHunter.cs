using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecruiterHunter : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;


    private bool isAbility;


    //for shooting
    private Camera cam;
    [SerializeField]
    private Transform Gun;
    [SerializeField]
    private Transform GunLight;

    //Components
    private PlayerMovement playerMove;

    Vector2 mousePos;

    //bullet
    public Transform Muzzle;
    public GameObject Muzzlefalsh;
    public GameObject bullet;
    public float bulletforce = 20f;

    private void Start()
    {
        cam = Camera.main;
        Muzzlefalsh.SetActive(false);
        playerMove = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!playerMove.PV.IsMine)
            return;

        IfAbility();



        //for shooting
        Aim();


        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
    
    }

    //Checking If Ability Is Turned On
    private void IfAbility()
    {
        if (isAbility)
        {
            //
        }
        else
        {
            playerMove.moveSpeed = normalMovementSpeed;
        }
    }

    void Aim()
    {
        Vector3 mousePosition = GetNouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        Gun.eulerAngles = new Vector3(0, 0, angle);

        Vector3 localScale = Vector3.one;

        if(angle > 90 || angle < -90)
        {
            localScale.y = -0.8f;
            GunLight.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else
        {
            localScale.y = +0.8f;
            GunLight.localRotation = Quaternion.Euler(0f, 0f, -90f);
        }

        Gun.localScale = localScale;
    }

    void Shoot()
    {
        StartCoroutine(_MuzzleFlash());
        GameObject Bullet = Photon.Pun.PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bullet"), Muzzle.position, Muzzle.rotation);
        Rigidbody2D bulletRB = Bullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(Muzzle.right * bulletforce, ForceMode2D.Impulse);
        GetComponent<CameraShake>().Shake(.05f, .1f);
    }
    IEnumerator _MuzzleFlash()
    {
        Muzzlefalsh.SetActive(true);
        yield return new WaitForSeconds(0.07f);
        Muzzlefalsh.SetActive(false);
    }

    //GetMouse Position in world z = 0f;
    private Vector3 GetNouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
    private Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    private Vector3 GetMouseWorldPositionWithZ(Vector3 sreenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(sreenPosition);
        return worldPosition;
    }


}
