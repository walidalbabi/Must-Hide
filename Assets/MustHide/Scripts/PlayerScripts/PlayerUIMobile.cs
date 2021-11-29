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
    private ShootingScript shootingScript;

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
    }
    public void TransformTo()
    {
        if (!propController.isProp && propController.canTransformTo && propController. propCol != null && PV.IsMine)
        {
            propController.TransformToProp();
            IncreaceHideTimeBtn.SetActive(true);
        }
        else
            if (propController.isProp && propController.canTransformTo)
            {
            propController.BackToTransformation(false);
            IncreaceHideTimeBtn.SetActive(false);
            }
    }

    public void IncHideTime()
    {
        propController.IncreaseHideTime();
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
                    shootingScript.Shoot(true);
                    break;
                case TouchPhase.Ended:
                    // do something when touch end like :
                    shootingScript.Shoot(false);
                    break;
            }
        }
    }

}
