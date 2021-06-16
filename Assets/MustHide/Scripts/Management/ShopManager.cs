using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{

    public static ShopManager instance;

    [SerializeField]
    private GameObject BuyPanel;
    private CanvasGroup BuyPanelCanvas;


    private ShopBtn shopBtn;

    [SerializeField]
    private Image characterImage;
    [SerializeField]
    private Text novaPriceTxt, eyesPriceTxt, charaterDescriptionTxt;

    private string characterDes;
    private float novaPrice;
    private float eyesPrice;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        BuyPanelCanvas = BuyPanel.GetComponent<CanvasGroup>();
    }

    public void Set_ShopBtnVar(ShopBtn var)
    {
        shopBtn = var;
        SetBuyPanel();
    }

    public void SetBuyPanel()
    {
        novaPrice = shopBtn.NovaPrice;
        eyesPrice = shopBtn.EyesPrice;
        characterDes = shopBtn.CharacterDescription;
        characterImage.sprite = shopBtn.characterImage.sprite;
        novaPriceTxt.text = novaPrice.ToString();
        eyesPriceTxt.text = eyesPrice.ToString();
        charaterDescriptionTxt.text = characterDes;

        ShowPanelUI();
    }

    public void ShowPanelUI()
    {
        if (BuyPanel.activeInHierarchy)
        {
            BuyPanel.SetActive(false);
        }
        else
        {
            BuyPanel.SetActive(true);
            BuyPanelCanvas.alpha = 0f;
            BuyPanelCanvas.LeanAlpha(1f, 0.3f);
        }
    }

    public void OnBuyPressed()
    {
        //Buy By Playfab
    }
}
