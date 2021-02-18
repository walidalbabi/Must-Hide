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
    public void StartErrorMsg(string txtError, bool isRestart, bool isReconnect)
    {
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        StopErrorMsg();
    }

    public void Reconnect()
    {
        Photon.Pun.PhotonNetwork.Reconnect();
        LoadingScript.instance.StartLoading("Reconnecting...");
        StopErrorMsg();
    }
}
