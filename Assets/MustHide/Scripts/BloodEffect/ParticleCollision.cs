using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    [SerializeField]
    private bool isMonster;
    ParticleSystem particle;

    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particle, other, collisionEvents);

        int count = collisionEvents.Count;
        count = Mathf.Clamp(count, 0, 2); 

        for (int i = 0; i < count; i++)
        {
            InstantiateBlood(i);
        }
    }

    private void InstantiateBlood(int count)
    {
        if (isMonster)
            Instantiate((GameObject)Resources.Load(Path.Combine("PhotonPrefabs", "Blood", "SplatRendererMonster")), collisionEvents[count].intersection, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        else
            Instantiate((GameObject)Resources.Load(Path.Combine("PhotonPrefabs", "Blood", "SplatRendererHunter")), collisionEvents[count].intersection, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
    }
}
