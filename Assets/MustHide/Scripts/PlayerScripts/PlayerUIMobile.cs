using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUIMobile : MonoBehaviour
{
    [SerializeField]
    private Photon.Pun.PhotonView PV;
    [SerializeField]
    private bool isHunter;
    [SerializeField]
    private PropsController propController;
    [SerializeField]
    private GameObject IncreaceHideTimeBtn;
    [SerializeField]
    private RecruiterHunter recruiterHunter;
    [SerializeField]
    private GameObject ShootBtn;
    [SerializeField]
    private GameObject ShootAndAimJoystick;
    [SerializeField]
    private Text AutoShootText;

    private bool autoShoot;
    private bool _canShoot;

    private void Start()
    {
        if (!PV.IsMine)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;

        if(!isHunter)
        if (propController.isProp)
            IncreaceHideTimeBtn.SetActive(true);
        else
            IncreaceHideTimeBtn.SetActive(false);
        else
        {
            
        }
    }
    public void TransformTo()
    {
        if (propController.isProp)
        {
            propController.BackToTransformation(false);


        }
        else
        {
            propController.TransformToProp();
        }
    }

    public void IncHideTime()
    {
        propController.IncreaseHideTime();
    }

    public void AutoShoot()
    {
        if (!autoShoot)
        {
            autoShoot = true;
            ShootBtn.SetActive(true);
            ShootAndAimJoystick.SetActive(false);
            AutoShootText.text = "Auto:On";
            recruiterHunter.AutoFire = true;
        }
        else
        {
            autoShoot = false;
            ShootBtn.SetActive(false);
            ShootAndAimJoystick.SetActive(true);
            AutoShootText.text = "Auto:OFF";
            recruiterHunter.AutoFire = false;
        }
    }
    public void Shoot()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Stationary:
                    // do something when touch began like :
                    recruiterHunter.Shoot(true);
                    break;
                case TouchPhase.Ended:
                    // do something when touch end like :
                    recruiterHunter.Shoot(false);
                    break;
            }
        }
    }

    public void StopShooting()
    {
       // recruiterHunter.Shoot(false);
    }
}
