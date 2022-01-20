using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Bright : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;
    [Tooltip("Numbers of Traps")]
    [SerializeField]
    private int HealBulletsCapacity;
    [Tooltip("How much time to use ability again")]
    [SerializeField]
    private float CoolDown;
    [Tooltip("Fire speed of the Gun")]
    [SerializeField]
    private float fireRate;
    [Tooltip("Magazin Capacity")]
    [SerializeField]
    private int magazin = 30;
    [Tooltip("Reload Time")]
    [SerializeField]
    private float reloadTime = 2f;

    [SerializeField]
    private Transform Muzzle;


    [SerializeField]
    private Text HealMagazin;

    public float NormalMovementSpeed { get { return normalMovementSpeed; } }

    public float FireRate { get { return fireRate; } }

    public int Magazin { get { return magazin; } }

    public float ReloadTime { get { return reloadTime; } }





    [HideInInspector]
    public bool isAbility;
    private bool isCoolDown;


    private float Counter;

    //Components
    [SerializeField]
    private Slider AbilitySlider;
    private PlayerMovement playerMove;
    private PhotonView PV;
    private Health healthScript;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMovement>();
    }
    private void Start()
    {
        healthScript = GetComponent<Health>();
        PV = GetComponent<PhotonView>();

        Counter = CoolDown;
        AbilitySlider.maxValue = CoolDown;
    }

    private void Update()
    {
        if (!playerMove.PV.IsMine)
            return;


        if (healthScript.isDead)
        {
            if (AbilitySlider.gameObject.activeInHierarchy)
                AbilitySlider.gameObject.SetActive(false);
            return;
        }

        HealMagazin.text = "Heal : " + HealBulletsCapacity;

        if (playerMove.isMoving)
        {
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.Running, 5f, 0, 0.05f, 1f, true);
        }


        if (Input.GetKeyUp(KeyCode.Space) && !healthScript.isDead)
        {
            SetAbilityOn();
        }

        if (!playerMove.enabled)
            isAbility = false;


        if (isCoolDown)
            Counter -= Time.deltaTime;

        AbilitySlider.value = Counter;

        //Slow Movement Down
        if (GetComponent<ShootingScript>().isReload || playerMove.isSlowingDown)
        {
            if (!healthScript.isDead)
            {
                GetComponent<ShootingScript>().anim.speed = 0.5f;
                playerMove.moveSpeed = normalMovementSpeed / 2f;
            }
            else
            {
                GetComponent<ShootingScript>().anim.speed = 1f;
                playerMove.moveSpeed = 6f;
            }
        }
        else
        {
            GetComponent<ShootingScript>().anim.speed = 1f;
            playerMove.moveSpeed = normalMovementSpeed;
        }

    }


    public void SetAbilityOn()
    {
        if(!isCoolDown && playerMove.enabled && healthScript.canUseAbility && HealBulletsCapacity > 0)
        {
            StartCoroutine(AbilityCooldown());
            IfAbility();
        }
    }

    //Checking If Ability Is Turned On
    private void IfAbility()
    {
        HealBulletsCapacity--;
     //   StartCoroutine(GetComponent<ShootingScript>()._MuzzleFlash());
        GameObject Bullet = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Hunters", "Abilities", "HealBullet"), Muzzle.position, Muzzle.rotation);
        GetComponent<CameraShake>().Shake(.05f, .1f);
        GetComponent<AudioManager>().PlaySound(AudioManager.Sound.GrenadeLauncher, 35f, 0, 1f, 1f, true);
    }

    //Ability Cooldown
    private IEnumerator AbilityCooldown()
    {
        isCoolDown = true;
        AbilitySlider.gameObject.SetActive(true);
        yield return new WaitForSeconds(CoolDown);
        isCoolDown = false;
        Counter = CoolDown;
        AbilitySlider.gameObject.SetActive(false);

    }
}
