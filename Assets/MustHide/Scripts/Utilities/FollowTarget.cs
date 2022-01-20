using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform Target;

    // Update is called once per frame
    void Update()
    {
        if (Target != null)
            transform.position = Target.position;
    }

    private void OnDisable()
    {
        Target = null;
    }

    public void SetTarget(int target)
    {
        Target = Photon.Pun.PhotonView.Find(target).transform;
    }

}
