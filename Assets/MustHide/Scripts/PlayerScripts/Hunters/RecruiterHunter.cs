using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class RecruiterHunter : MonoBehaviour
{
    [Tooltip("Normal speed of the player")]
    [SerializeField]
    private float normalMovementSpeed;
    [Tooltip("Fire speed of the Gun")]
    [SerializeField]
    private float fireRate;
    [Tooltip("Magazin Capacity")]
    [SerializeField]
    private int magazin = 30;
    [Tooltip("Reload Time")]
    [SerializeField]
    private float reloadTime = 2f;


    public float NormalMovementSpeed { get { return normalMovementSpeed; } }

    public float FireRate { get { return fireRate; } }

    public int Magazin { get { return magazin; } }

    public float ReloadTime { get { return reloadTime; } }


    private bool isAbility;




    //Components
    private PlayerMovement playerMove;

    private void Awake()
    {
        playerMove = GetComponent<PlayerMovement>();
    }
    private void Start()
    {

    }

    private void Update()
    {
        if (!playerMove.PV.IsMine)
            return;

        if (GetComponent<Health>().isDead)
            return;


        if (playerMove.isMoving)
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.Running, 5f, 0, 0.05f, 1f,true);

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
            
               
        }
    }

 




}
