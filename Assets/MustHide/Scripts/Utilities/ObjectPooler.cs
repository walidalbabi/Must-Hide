using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class ObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;


    private PhotonView PV;




    public static ObjectPooler instance;
    private void Awake()
    {

        instance = this;

        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i = 0; i < pool.size; i++)
            {
                
                GameObject Obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bullet"), Vector3.zero, Quaternion.identity);
                Obj.SetActive(false);
                objectPool.Enqueue(Obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }


    public GameObject SpawFromPool(string tag, Vector3 position, Quaternion rotation)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool With Tag " + tag +" Doens't Exicst.");
            return null;
        }

       GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObjects pooledObj = objectToSpawn.GetComponent<IPooledObjects>();

        if(pooledObj != null)
        {
           // pooledObj.OnObjectsSpawn();
        }


        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
 
}
