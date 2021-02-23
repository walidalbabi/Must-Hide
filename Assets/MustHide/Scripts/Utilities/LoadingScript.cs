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
    public void StartLoading(string txtLoading)
    {
        _Text.text = txtLoading;
        LoadingPanel.SetActive(true);
    }

    public void StopLoading()
    {
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
