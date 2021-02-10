using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RecruiterMonster : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;


    private bool isAbility;


    //Components
    private PlayerMovement playerMove;
    [SerializeField]
    private GameObject PlayerLight;

    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();



        if (!GetComponent<PhotonView>().IsMine)
            PlayerLight.SetActive(false);
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
