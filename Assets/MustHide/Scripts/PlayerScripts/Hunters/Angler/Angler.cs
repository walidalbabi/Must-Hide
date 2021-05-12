using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Angler : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;
    [Tooltip("Ability Duration | Also Asign that in the shield Prefab")]
    [SerializeField]
    private float Delay;
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
    private VolumeProfile[] volumeProfile;
    [SerializeField]
    private GameObject GunLight;

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

        if (playerMove.isMoving)
        {
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.Running, 5f, 0, 0.05f, 1f, true);
        }


        if (Input.GetKeyUp(KeyCode.Space) && !isAbility && !isCoolDown && playerMove.enabled && healthScript.canUseAbility)
        {
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.NightVision, 7f, 0, 1f, 1, true);
            StartCoroutine(AbilityDelay());
            StartCoroutine(AbilityCooldown());
        }

        if (!playerMove.enabled)
            isAbility = false;

        IfAbility();


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



    //Checking If Ability Is Turned On
    private void IfAbility()
    {


        if (isAbility)
        {

            if (!playerMove.isSlowingDown)
                playerMove.moveSpeed = NormalMovementSpeed;

            if (GunLight.activeInHierarchy)
                PV.RPC("RPC_WhenAbility", RpcTarget.AllBuffered, false);

            Camera.main.gameObject.GetComponent<Volume>().profile = volumeProfile[1];
        }
        else
        {

            if (!playerMove.isSlowingDown)
                playerMove.moveSpeed = NormalMovementSpeed;

            if (!GunLight.activeInHierarchy)
                PV.RPC("RPC_WhenAbility", RpcTarget.AllBuffered, true);

            Camera.main.gameObject.GetComponent<Volume>().profile = volumeProfile[0];

        }
    }


    //AbilityTime
    private IEnumerator AbilityDelay()
    {
        isAbility = true;
        yield return new WaitForSeconds(Delay);
        isAbility = false;
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

    [PunRPC]
    private void RPC_WhenAbility(bool activate)
    {
        GunLight.SetActive(activate);
    }

}
