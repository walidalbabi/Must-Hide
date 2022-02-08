using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayDeviceDependicies : MonoBehaviour
{

    [SerializeField] private GameObject[] _PCPanel;
    [SerializeField] private GameObject[] _MobilePanel;

    // Start is called before the first frame update
    void Start()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            foreach (var item in _PCPanel)
            {
                item.SetActive(true);
            }
        }
        else if(SystemInfo.deviceType == DeviceType.Handheld)
        {
            foreach (var item in _MobilePanel)
            {
                item.SetActive(true);
            }
        }
    }

  
}
