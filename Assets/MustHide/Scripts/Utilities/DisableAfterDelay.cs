using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour
{
    public float Delay;
    private void OnEnable()
    {
        Invoke("SetTime", Delay);

    }

    private void SetTime()
    {
        gameObject.SetActive(false);
    }
}
