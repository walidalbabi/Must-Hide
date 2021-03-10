using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShootingScript : MonoBehaviour
{
    private float fireRate;

    private int magazin;

    private float reloadTime;

    //for shooting

    public Transform Gun;
    [SerializeField]
    private Transform GunLight;

    [SerializeField]
    private Joystick AimJoystick;
    private int magazinCounter;
    private float counter;
    private bool canShoot;
    private float fireRateCounter;
    [HideInInspector]
    public bool isReload;

    //bullet
    public Transform Muzzle;
    public GameObject Muzzlefalsh;
    public bool AutoFire;


    
    public Animator anim;
    [SerializeField]
    private Slider ReloadTimeSlider;
    [SerializeField]
    private Text MagText;

    //Components
    private PlayerMovement playerMove;




    private void Awake()
    {
        playerMove = GetComponent<PlayerMovement>();

        if (GetComponent<RecruiterHunter>())
        {
            fireRate = GetComponent<RecruiterHunter>().FireRate;
            magazin = GetComponent<RecruiterHunter>().Magazin;
            reloadTime = GetComponent<RecruiterHunter>().ReloadTime;
        }
        else if (GetComponent<Angler>())
        {
            fireRate = GetComponent<Angler>().FireRate;
            magazin = GetComponent<Angler>().Magazin;
            reloadTime = GetComponent<Angler>().ReloadTime;
        }
        else if (GetComponent<Falcon>())
        {
            fireRate = GetComponent<Falcon>().FireRate;
            magazin = GetComponent<Falcon>().Magazin;
            reloadTime = GetComponent<Falcon>().ReloadTime;
        }
        else if (GetComponent<Fiddler>())
        {
            fireRate = GetComponent<Fiddler>().FireRate;
            magazin = GetComponent<Fiddler>().Magazin;
            reloadTime = GetComponent<Fiddler>().ReloadTime;
        }
        else if (GetComponent<Bright>())
        {
            fireRate = GetComponent<Bright>().FireRate;
            magazin = GetComponent<Bright>().Magazin;
            reloadTime = GetComponent<Bright>().ReloadTime;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Muzzlefalsh.SetActive(false);

        if (!GetComponent<Photon.Pun.PhotonView>().IsMine)
        {
            ReloadTimeSlider.gameObject.SetActive(false);
            MagText.gameObject.SetActive(false);
        }


        magazinCounter = magazin;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerMove.PV.IsMine)
            return;

        if (GetComponent<Health>().isDead)
            return;


        //for shooting
        Aim();
        Reload();

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
        }
        else if (SystemInfo.deviceType == DeviceType.Handheld && AutoFire)
        {
            RaycastHit2D hit = Physics2D.Raycast(GunLight.position, GunLight.transform.up, 7f);

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
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.MP5Reload, 20f, playerMove.PV.ViewID, 0.5f, 1f, true);
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


    void Aim()
    {

        if (SystemInfo.deviceType == DeviceType.Desktop)
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
        else if (SystemInfo.deviceType == DeviceType.Handheld)
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

        }

    }


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
            Bullet.GetComponent<BulletScript>().SetBulletPlayerHealth(GetComponent<Health>());
            GetComponent<CameraShake>().Shake(.05f, .1f);
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.MP5Shoot, 35f, 0, 1f, 1f, true);
        }

    }
    public IEnumerator _MuzzleFlash()
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
