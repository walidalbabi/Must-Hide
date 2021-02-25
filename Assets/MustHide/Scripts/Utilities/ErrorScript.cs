using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ErrorScript : MonoBehaviour
{
    [SerializeField]
    private Text _Text;
    [SerializeField]
    private GameObject CloseBtn;
    [SerializeField]
    private GameObject ErrorPanel;
    [SerializeField]
    private GameObject RestartGameBtn;
    [SerializeField]
    private GameObject ReconnectGameBtn;
    public static ErrorScript instance;


    private string ReconnectTo;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartErrorMsg(string txtError, bool isRestart, bool isReconnect, string whatToReconnect)
    {

        ReconnectTo = whatToReconnect;

        _Text.text = txtError;
        ErrorPanel.SetActive(true);

        if (isRestart)
            RestartGameBtn.SetActive(true);
        else
            RestartGameBtn.SetActive(false);

        if (isReconnect)
            ReconnectGameBtn.SetActive(true);
        else
            ReconnectGameBtn.SetActive(false);

        if (!isReconnect && !isRestart)
            CloseBtn.SetActive(true);

    }

    public void StopErrorMsg()
    {
        ErrorPanel.SetActive(false);
    }

    public void RestartGame()
    {

        NetworkManager.instance.DestroyObj();
        PlayFabLogin.instance.DestroyObj();
        VivoxManager.instance.DestroyObj();

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        StopErrorMsg();
    }

    public void Reconnect()
    {
        if (ReconnectTo == "photon")
        {
            Photon.Pun.PhotonNetwork.Reconnect();
            StartErrorMsg("Reconnecting...", false, false, "");
        }
        else if (ReconnectTo == "vivox")
            VivoxManager.instance.Login(Photon.Pun.PhotonNetwork.NickName, VivoxUnity.SubscriptionMode.Accept);

 
      
    }
}
