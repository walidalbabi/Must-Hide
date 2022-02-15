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
    [SerializeField] private A_ScaleOverTime _scaleAnim;

    private CanvasGroup _canvasGroup;
    private AudioSource _audioSource;


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


        _canvasGroup = ErrorPanel.GetComponent<CanvasGroup>();
        _audioSource = GetComponent<AudioSource>();
    }
    public void StartErrorMsg(string txtError, bool isRestart, bool isReconnect , bool isCloseErrorPanel, string whatToReconnect)
    {
        _audioSource.PlayOneShot(_audioSource.clip);
        ReconnectTo = whatToReconnect;
        _canvasGroup.LeanAlpha(1f, 0.3f);

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

        if (isCloseErrorPanel)
            CloseBtn.SetActive(true);
        else CloseBtn.SetActive(false);
    }

    public void StopErrorMsg()
    {
        StartCoroutine(StopL());
    }

    private IEnumerator StopL()
    {
        if (_canvasGroup != null)
            _canvasGroup.LeanAlpha(0f, 0.3f);
        if (_scaleAnim != null)
            _scaleAnim.ResetScaleToZero();
        yield return new WaitForSeconds(0.3f);
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
            StartErrorMsg("Reconnecting...", false, false, false,"");
        }
        else if (ReconnectTo == "vivox")
        {
            VivoxManager.instance.Logout();
            Invoke("LoginToVivox", 2f);
            StartErrorMsg("Reconnecting...", false, false, false,"");
        }
           
    }

    private void LoginToVivox()
    {
        VivoxManager.instance.Login(Photon.Pun.PhotonNetwork.NickName, VivoxUnity.SubscriptionMode.Accept);
    }
}
