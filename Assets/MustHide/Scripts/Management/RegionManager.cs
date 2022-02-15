using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
    public static RegionManager instance;

    [SerializeField] private GameObject _regionPanel;


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public bool CheckIfRegionSelected()
    {
        if (!PlayerPrefs.HasKey("HasRegion"))
        {
            return false;
        }

        return true;
    }

    public void ShowReigionPanel()
    {
        if (_regionPanel.activeInHierarchy) _regionPanel.SetActive(false);
        else _regionPanel.SetActive(true);
    }

    public void OnPressButtonRegion(string regionName)
    {
        PlayerPrefs.SetString("HasRegion", regionName);
        VivoxManager.instance.Logout();
        NetworkManager.instance.Disconnect();
        ShowReigionPanel();
    }
}
