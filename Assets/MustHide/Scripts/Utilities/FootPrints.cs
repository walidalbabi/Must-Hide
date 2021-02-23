using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class FootPrints : MonoBehaviour
{

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private ParticleSystem FootPrintparticle;
    private ParticleSystem.MainModule main;
    private ParticleSystem.ShapeModule shape;

    // Start is called before the first frame update
    void Start()
    {
     //  FootPrintparticle = GetComponent<ParticleSystem>();
        main = FootPrintparticle.main;
        shape = FootPrintparticle.shape;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;

        if (playerMovement.isMoving)
        {
            if (!FootPrintparticle.isPlaying)
                GetComponent<PhotonView>().RPC("RPC_FootPrintsEnabl", RpcTarget.OthersBuffered);

            if (playerMovement.movement.x > 0)
            {
                main.startRotationZ = -1.5707963268f;
                shape.scale = new Vector3(0.08f, 0.5f, 0.5f);
            }
            else if (playerMovement.movement.x < 0)
            {
                main.startRotationZ = 1.5707963268f;
                shape.scale = new Vector3(0.08f, 0.5f, 0.5f);
            }
            else if (playerMovement.movement.y > 0)
            {
                main.startRotationZ = 0f;
                shape.scale = new Vector3(0.5f, 0.08f, 0.5f);
            }
            else if (playerMovement.movement.y < 0)
            {
                main.startRotationZ = 3.1415926536f;
                shape.scale = new Vector3(0.5f, 0.08f, 0.5f);
            }
        }
        else
        {
            if (FootPrintparticle.isPlaying)
                GetComponent<PhotonView>().RPC("RPC_FootPrintsDisabl", RpcTarget.OthersBuffered);
        }


    }



    [PunRPC]
    private void RPC_FootPrintsEnable()
    {
            FootPrintparticle.Play();
    }

    [PunRPC]
    private void RPC_FootPrintsDisable()
    { 
            FootPrintparticle.Stop();
    }

}
