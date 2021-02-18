using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
public class FriendList : MonoBehaviourPunCallbacks
{



    private void OnEnable()
    {
        StartCoroutine(RefreshFriends());
    }

    private void OnDisable()
    {
        StopCoroutine(RefreshFriends());
    }

    IEnumerator RefreshFriends()
    {
        PlayFabLogin.instance.GetFriendList();

        yield return new WaitForSeconds(2f);
        StartCoroutine(RefreshFriends());
    }
}
