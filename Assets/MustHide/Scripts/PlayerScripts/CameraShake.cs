using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    private Camera cam;

    private float shakeAmount;


    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }
    public void Shake(float amount, float lenght)
    {

        shakeAmount = amount;
        InvokeRepeating("BeginShake", 0, 0.01f);
        Invoke("StopShake", lenght);
    }

    private void BeginShake()
    {
        if(shakeAmount > 0)
        {
            Vector3 camPos = cam.transform.position;
            float offesetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offesetY = Random.value * shakeAmount * 2 - shakeAmount;

            camPos.x += offesetX;
            camPos.y += offesetY;

            cam.transform.position = camPos;
        }
    }

    private void StopShake()
    {
        CancelInvoke("BeginShake");
      //  cam.transform.localPosition = Vector3.zero;
    }
}
