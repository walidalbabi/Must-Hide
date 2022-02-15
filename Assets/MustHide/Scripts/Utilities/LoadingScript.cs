using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingScript : MonoBehaviour
{
    [SerializeField]
    private Text _Text;
    [SerializeField]
    private Text _GameText;
    [SerializeField]
    private GameObject LoadingPanel;
    [SerializeField]
    private GameObject GameLoadingPanel;

    public static LoadingScript instance;

   [SerializeField] private A_ScaleOverTime _scaleAnim;

    private CanvasGroup _canvasGroup;
    private AudioSource _audioSource;
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

        _canvasGroup = LoadingPanel.GetComponent<CanvasGroup>();
        _audioSource = GetComponent<AudioSource>();
    }
    public void StartLoading(string txtLoading)
    {
        _audioSource.PlayOneShot(_audioSource.clip);
        _Text.text = txtLoading;
        LoadingPanel.SetActive(true);
        _canvasGroup.LeanAlpha(1f,0.3f);
    }

    public void StopLoading()
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
        LoadingPanel.SetActive(false);
    }

    public void StartGameLoading(string txtLoading)
    {
        _GameText.text = txtLoading;
        if (!GameLoadingPanel.activeInHierarchy)
            GameLoadingPanel.SetActive(true);
    }

    public void StopGameLoading()
    {
        GameLoadingPanel.SetActive(false);
    }
}
