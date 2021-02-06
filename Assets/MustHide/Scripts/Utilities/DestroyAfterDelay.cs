using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
  
    public float Delay;
    private void OnEnable()
    {
        Destroy(gameObject, Delay);
    }
}
