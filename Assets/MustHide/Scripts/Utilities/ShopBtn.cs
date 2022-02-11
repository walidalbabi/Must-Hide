using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBtn : MonoBehaviour
{
    public bool isMonster;
    public int index;
    public Image characterImage;
    public string CharacterDescription;


    public int EyesPrice { get { return _EyesPrice; } }
    public int NovaPrice { get { return _NovaPrice; } }


    private int _EyesPrice;
    private int _NovaPrice;

    [SerializeField]
    private Text novaTxt, eyesTxt;


    private void OnEnable()
    {
        //Get Price From Playfab
        Set_Nova(int.Parse(PlayFabLogin.instance.GetTitleDataVariable().Data[gameObject.name + "_Nova"]));
        Set_Eyes(int.Parse(PlayFabLogin.instance.GetTitleDataVariable().Data[gameObject.name + "_Eyes"]));
    }

    private void Set_Nova(int amount)
    {
        _NovaPrice = amount;

        //Set Amount On UI
        novaTxt.text = _NovaPrice.ToString();
    }

    private void Set_Eyes(int amount)
    {
        _EyesPrice = amount;

        //Set Amount On UI
        eyesTxt.text = _EyesPrice.ToString();
    }


    public void OnPressShowBuy()
    {
        ShopManager.instance.Set_ShopBtnVar(this);
    }
}
