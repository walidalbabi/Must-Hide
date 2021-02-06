using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallChecker : MonoBehaviour
{

    [SerializeField]
    private bool Top, Bottom, Left, Right;
    bool canWork;
    [SerializeField]
    private PropsController propsController;
    [SerializeField]
    private float X, Y;

    private void Start()
    {
        propsController = GetComponentInParent<PropsController>();
    }

    private void Update()
    {
        if (propsController.propCol != null && propsController.isProp && canWork)
            transform.parent.position = new Vector2(propsController.propCol.transform.position.x + X, propsController.propCol.transform.position.y + Y);
        else
        {
            X = 0;
            Y = 0;
        }

        if (propsController.isProp)
        {
            float Dis = Vector2.Distance(transform.parent.position, propsController.propCol.transform.position);

            if (Dis > 1f)
            {
                transform.parent.position = new Vector2(propsController.propCol.transform.position.x, propsController.propCol.transform.position.y);
            }
        }
  
       
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (!propsController.isProp)
            return;
        canWork = true;

        if (Top)
        {
            Y = -0.25f;
            X = 0;
            Debug.Log("Top");
        }
        else if (Bottom)
        {
            Y = 0.25f;
            X = 0;
            Debug.Log("Bottom");
        }
        else if (Left)
        {
            X = 0.25f;
            Y = 0f;
            Debug.Log("Left");
        }
        else if (Right)
        {
            X = -0.25f;
            Y = 0;
            Debug.Log("Right");
        }
        else
        {
            X = 0;
            Y = 0;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canWork = false;
    }
}
