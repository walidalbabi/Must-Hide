using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Recruiter : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;


    private bool isAbility;


    //Components
    private PlayerMovement playerMove;

    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();      
    }

    private void Update()
    {
        IfAbility();
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


}
