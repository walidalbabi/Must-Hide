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
    [SerializeField]
    private GameObject FootPrint;
    private ParticleSystem.MainModule main;
    //Components
    private PlayerMovement playerMove;
    [SerializeField]
    private GameObject PlayerLight;

    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();
        main = FootPrint.GetComponent<ParticleSystem>().main;
        if (!GetComponent<PhotonView>().IsMine)
        {
            PlayerLight.SetActive(false);
        }
         
    }

    private void Update()
    {
        if (playerMove.isMoving)
        {
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.Running, 2f, 0, 0.05f, true);
            if (!FootPrint.GetComponent<ParticleSystem>().isPlaying)
                GetComponent<PhotonView>().RPC("RPC_FootPrints", RpcTarget.AllBuffered, true);
      
            if (playerMove.movement.x > 0)
            {
                main.startRotationZ = -90f;
            }
            else if (playerMove.movement.x < 0)
            {
                main.startRotationZ = 90f;
            }
            else if (playerMove.movement.y > 0)
            {
                main.startRotationZ = 0f;
            }
            else if (playerMove.movement.y < 0)
            {
                main.startRotationZ = 180f;
            }
        }
        else
        {
            if(FootPrint.GetComponent<ParticleSystem>().isPlaying)
                GetComponent<PhotonView>().RPC("RPC_FootPrints", RpcTarget.AllBuffered, false);
        }
  

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


    [PunRPC]
    private void RPC_FootPrints(bool isActive)
    {
        if (!isActive)
            FootPrint.GetComponent<ParticleSystem>().Stop();
        else
            FootPrint.GetComponent<ParticleSystem>().Play();
    }


}
