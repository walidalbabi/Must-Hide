using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fascor : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;
    [Tooltip("Speed after ability")]
    [SerializeField]
    private float AbilitySpeed;
    [SerializeField]
    [Tooltip("Time of the ability")]
    private float Delay;
    [Tooltip("How much time to use ability again")]
    [SerializeField]
    private float CoolDown;

    private bool isAbility, isCoolDown;


    //Components

    private PlayerMovement playerMove;

    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && !isAbility && !isCoolDown && playerMove.enabled)
        {
            StartCoroutine(AbilityDelay());
            StartCoroutine(AbilityCooldown());
        }

        if (!playerMove.enabled)
            isAbility = false;

        IfAbility();
    }

    //Checking If Ability Is Turned On
    private void IfAbility()
    {
        if (isAbility)
        {
            playerMove.moveSpeed = AbilitySpeed;
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
        yield return new WaitForSeconds(CoolDown);
        isCoolDown = false;

    }

}
