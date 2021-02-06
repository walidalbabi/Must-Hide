using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohe : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;
    [Tooltip("Speed after ability")]
    [SerializeField]
    private float AbilitySpeed;
    [SerializeField]
    [Tooltip("Time of the ability||Note: Add Delay in the Ability GameObject Script")]
    private float Delay;
    [Tooltip("How much time to use ability again")]
    [SerializeField]
    private float CoolDown;

    [SerializeField]
    private GameObject HealAbilityObj;

    private bool  isCoolDown;


    //Components

    private PlayerMovement playerMove;

    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();

        playerMove.moveSpeed = normalMovementSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && !isCoolDown && playerMove.enabled)
        {
            StartCoroutine(AbilityCooldown());
            IfAbility();
        }

    }

    //Turn Ability On
    private void IfAbility()
    {
        Instantiate(HealAbilityObj, transform.position, Quaternion.identity);
    }

    //Ability Cooldown
    private IEnumerator AbilityCooldown()
    {
        isCoolDown = true;
        yield return new WaitForSeconds(CoolDown);
        isCoolDown = false;

    }

}