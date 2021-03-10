using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWebScript : MonoBehaviour
{

    [SerializeField]
    private ParticleSystem[] particle;
    [SerializeField]
    private float Delay;

    private void Start()
    {
        StartCoroutine(StartSpiderWeb());
    }



    private void StopSpiderWeb()
    {
        foreach (var part in particle)
        {
            part.Stop();
        }

        Invoke("DisableLightAfter", 1f);
    }

    private void DisableLightAfter()
    {
        gameObject.SetActive(false);
    }

    IEnumerator StartSpiderWeb()
    {
        foreach (var part in particle)
        {
            part.Play();
        }
        yield return new WaitForSeconds(Delay);
        StopSpiderWeb();
    }
}
