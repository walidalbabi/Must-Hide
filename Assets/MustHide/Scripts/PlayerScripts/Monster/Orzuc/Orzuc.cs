using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Orzuc : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;
    [Tooltip("Speed after ability")]
    [SerializeField]
    private float AbilitySpeed;

    [Tooltip("How much time to use ability again")]
    [SerializeField]
    private float CoolDown;

    private float Counter;

    private bool isAbility, isCoolDown;

    //Foot Prints
    private float _footPrintsCounter;

    private float _footPrintsTime = 0.1f;

    private int feetIndex = 1;

    //Components
    [SerializeField]
    private Slider AbilitySlider;
    private Health healthScript;
    private PlayerMovement playerMove;

    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();
        healthScript = GetComponent<Health>();
        Counter = CoolDown;
        AbilitySlider.maxValue = CoolDown;

    }

    private void Update()
    {

        if (!playerMove.PV.IsMine)
            return;

        if (playerMove.isMoving)
        {
            if (healthScript.isDead)
            {
                if (AbilitySlider.gameObject.activeInHierarchy)
                    AbilitySlider.gameObject.SetActive(false);
                return;
            }

            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.Running, 2f, 0, 0.05f, 1f, true);
            FootPrint();

        }

        if (Input.GetKeyUp(KeyCode.Space) && !isCoolDown && playerMove.enabled && healthScript.canUseAbility)
        {
            StartCoroutine(AbilityCooldown());
            IfAbility();
        }



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
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Monsters", "Abilities", "SpiderWeb"), transform.position, Quaternion.identity);
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

