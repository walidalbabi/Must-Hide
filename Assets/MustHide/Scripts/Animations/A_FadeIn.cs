using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_FadeIn : MonoBehaviour
{

    [SerializeField] private float _time;

    private CanvasGroup _canvasGroup;


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartFadeIn();
    }

    private void StartFadeIn()
    {
        _canvasGroup.LeanAlpha(1f, _time);
    }
}
