using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_ScaleOverTime : MonoBehaviour
{

    [SerializeField] private float _time;
    [SerializeField] private Vector3 _targetScale;
   
    void Start()
    {
        StartScale();
    }


    private void StartScale()
    {
        transform.LeanScale(_targetScale, _time);
    }


}
