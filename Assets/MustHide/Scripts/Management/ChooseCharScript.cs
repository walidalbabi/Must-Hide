using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseCharScript : MonoBehaviour
{
    public static ChooseCharScript instance;

    public string Name;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    public void SetCharName(string name)
    {
        Name = name;
    }
}
