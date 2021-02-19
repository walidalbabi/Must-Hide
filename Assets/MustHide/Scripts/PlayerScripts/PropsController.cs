using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Photon.Pun;
using UnityEngine.UI;
public class PropsController : MonoBehaviour
{
    [HideInInspector]
    public float counter = 0;
    [HideInInspector]
    public float buffCounter = 0;


    [SerializeField]
    private GameObject sprite;
    private BoxCollider2D col;
    private PhotonView PV;

    [HideInInspector]
    public bool isProp = false;
    private bool canTransformTo;

    [SerializeField]
    private float timeToTransformBack = 10f;
    private bool isCount;

    [SerializeField]
    private float noTransformBuff = 3f;
    [HideInInspector]
    public bool isBuff;

    //Props Components
    [HideInInspector]
    public BoxCollider2D propCol;

    private float closerDis;

    private List<Transform> colsPos = new List<Transform>();
    private List<float> dis = new List<float>();

    private SpriteRenderer sr;


    [SerializeField]
    private Slider HideTimeSlider;
    [SerializeField]
    private Slider BuffTimeSlider;

    
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        PV = GetComponent<PhotonView>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
            return;

        CheckIfIamAProp();
           

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isProp && canTransformTo && propCol != null && PV.IsMine)
            {
                TransformToProp();
            }else
            if (isProp)
            {
                BackToTransformation(false);
            }
        }
    }

    private void CheckIfIamAProp()
    {
        if (!isProp)
        {
            if (propCol != null)
                CheckProps();
            CalculateDistance();

            //Check Buff
            if (isBuff)
            {
                BuffTimeSlider.gameObject.SetActive(true);
                BuffTimeSlider.maxValue = noTransformBuff;
                BuffTimeSlider.value = buffCounter;
                CountBuffFunction();
            }
            else
            {
                BuffTimeSlider.gameObject.SetActive(false);
            }
     

            if (buffCounter < noTransformBuff && isBuff)
                canTransformTo = false;
            else if (buffCounter >= noTransformBuff && isBuff)
            {
                canTransformTo = true;
                isBuff = false;
                buffCounter = 0;
            }

        }
        else
        {
            //Check Hide Time
            if (isCount)
            {
                HideTimeSlider.maxValue = timeToTransformBack;
                HideTimeSlider.value = counter;
                CountFunction();
            }
       

            if (counter >= timeToTransformBack)
                BackToTransformation(false);


            if (Input.GetKeyDown(KeyCode.Q))
            {
                IncreaseHideTime();
            }

            transform.position = transform.position;

        }
    }
    private void CountFunction()
    {
        counter += Time.deltaTime;
    }
    private void CountBuffFunction()
    {
        buffCounter += Time.deltaTime;
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

    public void IncreaseHideTime()
    {
        GetComponent<AudioManager>().PlaySound(AudioManager.Sound.HideIncreaseSound, 15f, 0, 1f, true);
        counter = 0f;
    }
    //Change Back to you transformation

    public void BackToTransformation(bool forced)
    {
        if (propCol == null)
            return;
        HideTimeSlider.gameObject.SetActive(false);
        //Its not a prop anymore
        isBuff = true;
        canTransformTo = true;
        isProp = false;
        isCount = false;
        counter = 0;
        //set all properties back to default
        PV.RPC("OnTransfering", RpcTarget.AllBuffered, false);

        GetComponent<PlayerMovement>().enabled = true;

        propCol.GetComponent<PhotonView>().TransferOwnership(0);
      //  propCol.GetComponent<BlocksScript>().SetPropController(null);
        propCol.GetComponent<PropEnableDisableComponents>().OnPropDeselected();
        propCol = null;

        if(forced)
            GetComponent<AudioManager>().PlaySound(AudioManager.Sound.RightProp, 10f, 0, 1f, true);

    }


    //Change  to a prop
    public void TransformToProp()
    {
        if (propCol == null)
            return;
        HideTimeSlider.gameObject.SetActive(true);
        canTransformTo = false;
        //Its  a prop now!
        isProp = true;
        isCount = true;
        //Get prop components and set it to props control properties

        PV.RPC("OnTransfering", RpcTarget.AllBuffered, true);

        if (sr != null)
            sr.color = Color.white;

        GetComponent<AudioManager>().PlaySound(AudioManager.Sound.MonsterTransform, 10f, 0, 1f, true);

        GetComponent<PlayerMovement>().isMoving = false;
        GetComponent<PlayerMovement>().enabled = false;
        propCol.GetComponent<PhotonView>().TransferOwnership(PV.OwnerActorNr);
        propCol.GetComponent<PropEnableDisableComponents>().OnPropSelected();
        Invoke("SetPropController", 0.2f);
    }

    private void SetPropController()
    {
        propCol.GetComponent<BlocksScript>().SetPropController(this);
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
        }
        else
        {
            sprite.SetActive(true);
            col.enabled = true;
        }
    }
}

