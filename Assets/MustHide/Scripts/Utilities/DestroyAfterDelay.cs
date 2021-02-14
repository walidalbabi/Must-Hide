using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
  
    public float Delay;
    private void OnEnable()
    {
        Invoke("SetTime", 0.5f);

    }

    private void SetTime()
    {
        Destroy(gameObject, Delay);
    }
}
