using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class RecruiterHunter : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;
    [Tooltip("Fire speed of the Gun")]
    [SerializeField]
    private float fireRate;
    [Tooltip("Magazin Capacity")]
    [SerializeField]
    private int magazin = 30;
    [Tooltip("Reload Time")]
    [SerializeField]
    private float reloadTime = 2f;
    [Tooltip("Bullet Speed/Force")]
    [SerializeField]
    private float bulletforce = 20f;


    //for shooting
    [SerializeField]
    private Transform Gun;
    [SerializeField]
    private Transform GunLight;
    private int magazinCounter;
    private float counter;
    private bool canShoot;
    private float fireRateCounter;


    private bool isAbility, isReload;


    //bullet
    public Transform Muzzle;
    public GameObject Muzzlefalsh;
    public GameObject bullet;
    bool coroutinRunning;


    //Components
    private PlayerMovement playerMove;
    [SerializeField]
    private Slider ReloadTimeSlider;
    [SerializeField]
    private Text MagText;
    private void Awake()
    {
        playerMove = GetComponent<PlayerMovement>();
    }
    private void Start()
    {
        Muzzlefalsh.SetActive(false);

        if (!playerMove.PV.IsMine)
        {
            ReloadTimeSlider.gameObject.SetActive(false);
            MagText.gameObject.SetActive(false);
        }


        magazinCounter = magazin;
    }

    private void Update()
    {
        if (!playerMove.PV.IsMine)
            return;

        IfAbility();
        //for shooting
        Aim();
        Reload();

        if (Input.GetButton("Fire1") && !isReload)
        {
            Debug.Log("Fire");
            Shoot();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Stop Fire");
            if (Muzzlefalsh.activeInHierarchy)
                Muzzlefalsh.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
    
    }

    private void Reload()
    {
        MagText.text = magazinCounter + "/" + magazin;

        Mathf.Clamp(magazinCounter, 0, magazin);

        //Checking Magazin
        if (magazinCounter <= 0)
            isReload = true;
        //Start Reload Time
        if (isReload)
        {
            ReloadTimeSlider.gameObject.SetActive(true);
            ReloadTimeSlider.maxValue = reloadTime;
            ReloadTimeSlider.value = counter;
            counter += Time.deltaTime;
        }
        //Wen Reload End
        if (counter >= reloadTime)
        {
            magazinCounter = magazin;
            ReloadTimeSlider.gameObject.SetActive(false);
            isReload = false;
            counter = 0;
        }
         
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
        if (!isReload)
        {
            //FireRate Counting
            fireRateCounter += Time.deltaTime;
            if (fireRateCounter >= fireRate)
            {
                canShoot = true;
                fireRateCounter = 0;
            }


            if (!canShoot)
                return;



            //Shooting
            canShoot = false;
            magazinCounter--;
            StartCoroutine(_MuzzleFlash());
            GameObject Bullet = Photon.Pun.PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bullet"), Muzzle.position, Muzzle.rotation);
            Bullet.GetComponent<BulletScript>().SetBulletPlayerHealth(GetComponent<Health>());
            Rigidbody2D bulletRB = Bullet.GetComponent<Rigidbody2D>();
            bulletRB.AddForce(Muzzle.right * bulletforce, ForceMode2D.Impulse);
            GetComponent<CameraShake>().Shake(.05f, .1f);
        }

    }
    IEnumerator _MuzzleFlash()
    {
        Muzzlefalsh.SetActive(true);
        yield return new WaitForSeconds(fireRate / 3f);
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
