using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadingScript : MonoBehaviour
{
    [SerializeField]
    private Text _Text;
    [SerializeField]
    private GameObject LoadingPanel;

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
}
