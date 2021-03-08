using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class HealArea : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] particle;
    [SerializeField]
    private GameObject healAreaLight;
    [SerializeField]
    private float Delay;

    private void Start()
    {
        StartCoroutine(StartHealArea());

    }

    private void Update()
    {

    }


    private void StopHealArea()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        foreach (var part in particle)
        {
            part.Stop();
        }

        Invoke("DisableLightAfter", 1f);
        GetComponent<AudioSource>().Pause();
    }

    private void DisableLightAfter()
    {
        Destroy(gameObject);
    }

    IEnumerator StartHealArea()
    {
        yield return new WaitForSeconds(Delay);
        StopHealArea();
    }
}
