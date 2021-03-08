using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class Bomog : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;
    [Tooltip("Speed after ability")]
    [SerializeField]
    private float AbilitySpeed; 
    [Tooltip("Ability Duration | Also Asign that in the shield Prefab")]
    [SerializeField]
    private float Delay;
    [Tooltip("How much time to use ability again")]
    [SerializeField]
    private float CoolDown;
    [SerializeField]
    private GameObject ShieldEffect;


    [HideInInspector]
    public bool isAbility;
    private bool isCoolDown;


    private float Counter;

    //Foot Prints
    private float _footPrintsCounter;

    private float _footPrintsTime = 0.1f;

    private int feetIndex = 1;

    //Components
    [SerializeField]
    private Slider AbilitySlider;
    private PlayerMovement playerMove;
    private PhotonView PV;
    private Health healthScript;

    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();
        healthScript = GetComponent<Health>();
        PV = GetComponent<PhotonView>();

        Counter = CoolDown;
        AbilitySlider.maxValue = CoolDown;
    }

    private void Update()
    {
        if (playerMove.isMoving)
        {
            if (GetComponent<Health>().isDead)
            {
                if (AbilitySlider.gameObject.activeInHierarchy)
                    AbilitySlider.gameObject.SetActive(false);
                return;
            }

            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.Running, 2f, 0, 0.05f, 1f, true);
            FootPrint();

        }


        if (Input.GetKeyUp(KeyCode.Space) && !isAbility && !isCoolDown && playerMove.enabled)
        {
            StartCoroutine(AbilityDelay());
            StartCoroutine(AbilityCooldown());
        }

        if (!playerMove.enabled)
            isAbility = false;

        IfAbility();


        if (isCoolDown)
            Counter -= Time.deltaTime;

        AbilitySlider.value = Counter;
    }

    private void FootPrint()
    {
        //Foot Prints
        if (_footPrintsCounter >= _footPrintsTime)
        {
            if (playerMove.movement.x > 0)
            {
                if (feetIndex == 1)
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FootPrintsObj"), transform.position + new Vector3(0f, -0.4f, 0f), Quaternion.Euler(0f, 0f, -90f));
                else if (feetIndex == 2)
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FootPrintsObj"), transform.position + new Vector3(0f, -0.5f, 0f), Quaternion.Euler(0f, 0f, -90f));
            }
            else if (playerMove.movement.x < 0)
            {
                if (feetIndex == 1)
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FootPrintsObj"), transform.position + new Vector3(0f, -0.4f, 0f), Quaternion.Euler(0f, 0f, 90f));
                else if (feetIndex == 2)
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FootPrintsObj"), transform.position + new Vector3(0f, -0.5f, 0f), Quaternion.Euler(0f, 0f, 90f));
            }
            else if (playerMove.movement.y > 0)
            {
                if (feetIndex == 1)
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FootPrintsObj"), transform.position + new Vector3(-0.1f, -0.4f, 0f), Quaternion.Euler(0f, 0f, 0f));
                else if (feetIndex == 2)
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FootPrintsObj"), transform.position + new Vector3(0.1f, -0.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
            }
            else if (playerMove.movement.y < 0)
            {
                if (feetIndex == 1)
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FootPrintsObj"), transform.position + new Vector3(-0.1f, -0.4f, 0f), Quaternion.Euler(0f, 0f, 180f));
                else if (feetIndex == 2)
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "FootPrintsObj"), transform.position + new Vector3(0.1f, -0.5f, 0f), Quaternion.Euler(0f, 0f, 180f));
            }

            if (feetIndex == 1)
                feetIndex = 2;
            else
                feetIndex = 1;

            _footPrintsCounter = 0;
        }
        else
        {
            _footPrintsCounter += Time.deltaTime;
        }
    }

    //Checking If Ability Is Turned On
    private void IfAbility()
    {
 

        if (isAbility)
        {
            playerMove.moveSpeed = AbilitySpeed;

            if (!ShieldEffect.activeInHierarchy)
                PV.RPC("RPC_SetShieldObj", RpcTarget.AllBuffered);
        }
        else
        {
            playerMove.moveSpeed = normalMovementSpeed;

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
    private void RPC_SetShieldObj()
    {
        ShieldEffect.SetActive(true);
    }

}
