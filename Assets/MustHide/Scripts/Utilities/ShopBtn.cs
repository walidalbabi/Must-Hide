using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBtn : MonoBehaviour
{

    public Image characterImage;
    public string CharacterDescription;


    public float EyesPrice { get { return _EyesPrice; } }
    public float NovaPrice { get { return _NovaPrice; } }


    private float _EyesPrice;
    private float _NovaPrice;

    [SerializeField]
    private Text novaTxt, eyesTxt;


    private void Start()
    {
        //Get Price From Playfab
        Set_Nova(6000);
        Set_Eyes(250);

     
   
    }

    private void Set_Nova(float amount)
    {
        _NovaPrice = amount;

        //Set Amount On UI
        novaTxt.text = _NovaPrice.ToString();
    }

    private void Set_Eyes(float amount)
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
