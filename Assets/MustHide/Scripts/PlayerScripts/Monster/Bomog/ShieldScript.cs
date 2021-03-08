using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private ParticleSystem[] particle;
    [SerializeField]
    private float Delay;

    private void OnEnable()
    {
        StartCoroutine(StartHealArea());

    }

    private void Update()
    {
        transform.position = parent.position;
    }

    private void StopHealArea()
    {
        foreach (var part in particle)
        {
            part.Stop();
        }

        Invoke("DisableLightAfter", 1f);
        GetComponent<AudioSource>().Stop();
    }

    private void DisableLightAfter()
    {
        gameObject.SetActive(false);
    }

    IEnumerator StartHealArea()
    {
        foreach (var part in particle)
        {
            part.Play();
        }
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(Delay);
        StopHealArea();
    }
}
