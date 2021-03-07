using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    private float _footPrintsCounter;
    [SerializeField]
    private float _footPrintsTime = 0.2f;
    [SerializeField]
    int feetIndex = 1;
    private void Start()
    {
        playerMove = GetComponent<PlayerMovement>();         
    }

    private void Update()
    {
        if (playerMove.isMoving)
        {
            if (GetComponent<Health>().isDead)
                return;

            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.Running, 2f, 0, 0.05f, 1f,true);


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
