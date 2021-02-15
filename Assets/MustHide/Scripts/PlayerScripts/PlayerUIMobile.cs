using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIMobile : MonoBehaviour
{
    [SerializeField]
    private PropsController propController;
    [SerializeField]
    private GameObject IncreaceHideTimeBtn;


    private void Update()
    {
        if (!GetComponent<Photon.Pun.PhotonView>().IsMine)
            return;

        if (GetComponent<PropsController>().isProp)
            IncreaceHideTimeBtn.SetActive(true);
        else
            IncreaceHideTimeBtn.SetActive(false);
    }
    public void TransformTo()
    {
        if (GetComponent<PropsController>().isProp)
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
}
