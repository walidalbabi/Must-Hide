using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedPrefabs : MonoBehaviour
{
    public GameObject Prefab;
    public string Path;

    public NetworkedPrefabs(GameObject obj, string path)
    {
        Prefab = obj;
        Path = path;
    }
}
