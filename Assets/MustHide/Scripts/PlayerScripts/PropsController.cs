using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Photon.Pun;

public class PropsController : MonoBehaviour
{
   // [HideInInspector]
    public float counter = 0;


    [SerializeField]
    private GameObject sprite;
    private BoxCollider2D col;
    private PlayerMovement PM;
    private PhotonView PV;

    [HideInInspector]
    public bool isProp = false;
    private bool canTransformTo;

    [SerializeField]
    private float timeToTransformBack = 10f;
    private bool isCount;

    [SerializeField]
    private AudioClip[] sounds;
    private AudioSource audioSource;

    //Props Components
    [HideInInspector]
    public BoxCollider2D propCol;
    private GameObject currentSelectedProp;
    private float closerDis;

    private List<Transform> colsPos = new List<Transform>();
    private List<float> dis = new List<float>();

    private SpriteRenderer sr;

    
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        PM = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
        PV = GetComponent<PhotonView>();

     
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

        if (!isProp)
        {
            if (propCol != null)
                CheckProps();
            CalculateDistance();
        }
        else
        {
            if (isCount)
                CountFunction();

            if (counter >= timeToTransformBack)
                BackToTransformation();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
                counter = 0f;
            }

        }
           

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isProp && canTransformTo && propCol != null && PV.IsMine)
            {
                TransformToProp();
            }else
            if (isProp)
            {
                BackToTransformation();
            }
        }
    }


    private void CountFunction()
    {
        counter += Time.deltaTime;

    }

    private void CheckProps()
    {

        if(colsPos.Count > 0)
        {
            for (int i = 0; i < colsPos.Count; i++)
            {
                if (propCol == colsPos[i].gameObject.GetComponent<BoxCollider2D>())
                {
                    return;
                }
                else
                {
                    propCol = null;
                    canTransformTo = false;
                    if (sr != null)
                        sr.color = Color.white;
                }

            }
        }
        else
        {
            if (sr != null)
                sr.color = Color.white;
            propCol = null;
            canTransformTo = false;
        }
        
    }


    //Change Back to you transformation

    public void BackToTransformation()
    {
        //Its not a prop anymore
        canTransformTo = true;
        isProp = false;
        isCount = false;
        counter = 0;
        //set all properties back to default
        PV.RPC("OnTransfering", RpcTarget.AllBuffered, false);
  

        propCol.GetComponent<BlocksScript>().enabled = false;
        propCol.GetComponent<PhotonTransformView>().enabled = true;
        propCol.GetComponent<PhotonView>().TransferOwnership(0);

        propCol = null;


    }


    //Change  to a prop
    public void TransformToProp()
    {
        canTransformTo = false;
        //Its  a prop now!
        isProp = true;
        isCount = true;
        //Get prop components and set it to props control properties

        PV.RPC("OnTransfering", RpcTarget.AllBuffered, true);

        if (sr != null)
            sr.color = Color.white;


        propCol.GetComponent<BlocksScript>().enabled = true;
        propCol.GetComponent<PhotonTransformView>().enabled = false;
        propCol.GetComponent<PhotonView>().TransferOwnership(PV.OwnerActorNr);
        transform.position = new Vector2(propCol.transform.position.x, propCol.transform.position.y - 0.25f);

    }


    //Calculate Distance to find witch prop is the closest
    private void CalculateDistance()
    {
        if (colsPos.Count == 0)
            return;

        //Get evry transform Distance
        for (int i = 0; i < colsPos.Count; i++)
        {
            if (colsPos[i] != null)
                dis[i] = Vector2.Distance(transform.position, colsPos[i].position);
        }

        for (int i = 0; i < dis.Count; i++)
        {

              if (dis[i] < 1.9f)
              {

                    if (dis[i] < closerDis)
                    {
                        closerDis = dis[i];
                        propCol = null;
                  
                    }

                    if (sr != null)
                        sr.color = Color.white;

                propCol = colsPos[i].gameObject.GetComponent<BoxCollider2D>();
                    if (propCol.GetComponent<BlocksScript>().isActiveAndEnabled == false)
                    {
                        sr = propCol.gameObject.GetComponent<SpriteRenderer>();
                        sr.color = Color.green;

                        canTransformTo = true;
                    }

              }    
        }
    }

    //Change To a Prop
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Props"))
        {
            if (isProp)
                return;

            //Check if List Contain the new Transform before adding it
            for (int i = 0; i < colsPos.Count; i++)
            {
                if (colsPos[i] == collision.gameObject.transform)
                    return;
            }
            //Add the new Transform
                colsPos.Add(collision.gameObject.transform);
                dis.Add(1);
          
        }
    }

    //when the props are no more in your range
    private void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < colsPos.Count; i++)
        {
            if (colsPos[i] == collision.gameObject.transform)
            {
               
                colsPos.Remove(collision.gameObject.transform);

              
                dis.RemoveAt(i);
            }       
        }
    }
   
   [PunRPC]
   private void OnTransfering(bool toProp)
    {
        if (toProp)
        {
            sprite.SetActive(false);
            col.enabled = false;
            propCol.GetComponent<PropSyncFix>().enabled = false;
        }
        else
        {
            sprite.SetActive(true);
            col.enabled = true;
            propCol.GetComponent<PropSyncFix>().enabled = true;
        }
    }
}

