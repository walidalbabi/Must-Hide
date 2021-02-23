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
    [SerializeField]
    private Joystick AimAndShootJoystick;
    [SerializeField]
    private Joystick AimJoystick;
    private int magazinCounter;
    private float counter;
    private bool canShoot;
    private float fireRateCounter;


    private bool isAbility, isReload;


    //bullet
    public Transform Muzzle;
    public GameObject Muzzlefalsh;
    public GameObject bullet;
    public bool AutoFire;

    //Components
    private PlayerMovement playerMove;
    [SerializeField]
    private Slider ReloadTimeSlider;
    [SerializeField]
    private Text MagText;
    private void Awake()
    {
        playerMove = GetComponent<PlayerMovement>();

        objectPooler = ObjectPooler.instance;
    }
    private void Start()
    {
        Muzzlefalsh.SetActive(false);

        if (!GetComponent<Photon.Pun.PhotonView>().IsMine)
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

        if(playerMove.isMoving)
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.Running, 5f, 0, 0.05f, true);

        IfAbility();

        //for shooting
        Aim();
        Reload();
     //   Debug.DrawRay(GunLight.position, GunLight.transform.up, Color.red);
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (Input.GetButton("Fire1") && !isReload)
            {
                Shoot(true);
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (Muzzlefalsh.activeInHierarchy)
                    Muzzlefalsh.SetActive(false);
            }
        }else if (SystemInfo.deviceType == DeviceType.Handheld && AutoFire)
        {
            RaycastHit2D hit = Physics2D.Raycast(GunLight.position, GunLight.transform.up, 5f);
          
            if (hit.collider.gameObject.CompareTag("Monster"))
            {
                Shoot(true);
            }
            else
            {
                if (Muzzlefalsh.activeInHierarchy)
                    Muzzlefalsh.SetActive(false);
            }
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
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.MP5Reload, 20f, playerMove.PV.ViewID, 0.5f, true);
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

        if(SystemInfo.deviceType == DeviceType.Desktop)
        {
            Vector3 mousePosition = GetNouseWorldPosition();

            Vector3 aimDirection = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            Gun.eulerAngles = new Vector3(0, 0, angle);

            Vector3 localScale = Vector3.one;

            if (angle > 90 || angle < -90)
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
        else if(SystemInfo.deviceType == DeviceType.Handheld)
        {
  
            //Aim Joystick
            float xMovementRightJoystick = AimJoystick.Horizontal; // The horizontal movement from joystick 02
            float yMovementRightJoystick = AimJoystick.Vertical; // The vertical movement from joystick 02

            if (xMovementRightJoystick != 0 || yMovementRightJoystick != 0)
            {
                // calculate the player's direction based on angle
                float tempAngle = Mathf.Atan2(yMovementRightJoystick, xMovementRightJoystick) * Mathf.Rad2Deg;

                Gun.eulerAngles = new Vector3(0, 0, tempAngle);
                Vector3 localScale = Vector3.one;

                if (tempAngle > 90 || tempAngle < -90)
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


            //Aim And Shoot Joystick
            float xMovementRightJoystick1 = AimAndShootJoystick.Horizontal; // The horizontal movement from joystick 02
            float yMovementRightJoystick1 = AimAndShootJoystick.Vertical; // The vertical movement from joystick 02

            if (xMovementRightJoystick1 != 0 || yMovementRightJoystick1 != 0)
            {
                if (!isReload)
                    Shoot(true);
               
                // calculate the player's direction based on angle
                float tempAngle1 = Mathf.Atan2(yMovementRightJoystick1, xMovementRightJoystick1) * Mathf.Rad2Deg;

                Gun.eulerAngles = new Vector3(0, 0, tempAngle1);
                Vector3 localScale1 = Vector3.one;

                if (tempAngle1 > 90 || tempAngle1 < -90)
                {
                    localScale1.y = -0.8f;
                    GunLight.localRotation = Quaternion.Euler(0f, 0f, 90f);
                }
                else
                {
                    localScale1.y = +0.8f;
                    GunLight.localRotation = Quaternion.Euler(0f, 0f, -90f);
                }

                Gun.localScale = localScale1;
            }
            else
            {
                if (Muzzlefalsh.activeInHierarchy)
                    Muzzlefalsh.SetActive(false);
            }
             

        }



    }
    ObjectPooler objectPooler;
    public void Shoot(bool isCanShoot)
    {
        if (!isCanShoot)
            return;

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
          // GameObject Bullet = objectPooler.SpawFromPool("Bullet", Muzzle.position, Muzzle.rotation);
             Bullet.GetComponent<BulletScript>().SetBulletPlayerHealth(GetComponent<Health>());
            //Rigidbody2D bulletRB = Bullet.GetComponent<Rigidbody2D>();
            //bulletRB.AddForce(Muzzle.right * bulletforce, ForceMode2D.Impulse);
            GetComponent<CameraShake>().Shake(.05f, .1f);
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.MP5Shoot, 35f, 0, 1f, true);
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
