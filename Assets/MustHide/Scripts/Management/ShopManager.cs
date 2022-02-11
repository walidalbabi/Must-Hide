using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private Text novaPriceTxt, eyesPriceTxt;
    [SerializeField] private TextMeshProUGUI charaterDescriptionTxt;

    private string characterDes;
    private int novaPrice;
    private int eyesPrice;
    

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

    public void OnBuyWithNovaPressed()
    {
        //Buy By Playfab
       // PlayfabCloudSaving.instance.GetUserData(PlayFabLogin.instance.PlayerInfo.AccountInfo.PlayFabId);
        if(PlayfabCloudSaving.instance._Nova >= novaPrice)
        {
            if (shopBtn.isMonster)
            {
                MenuManager.instance.UnlockMonsterNova(shopBtn.index, novaPrice);
            }
            else
            {
                MenuManager.instance.UnlockHuntersNova(shopBtn.index, novaPrice);
            }
        }
        ShowPanelUI();
    }

    public void OnBuyWithEyesPressed()
    {
        //Buy By Playfab
        //PlayfabCloudSaving.instance.GetUserData(PlayFabLogin.instance.PlayerInfo.AccountInfo.PlayFabId);
        if (PlayfabCloudSaving.instance._Eyes >= eyesPrice)
        {
            if (shopBtn.isMonster)
            {
                MenuManager.instance.UnlockMonsterEyes(shopBtn.index, eyesPrice);
            }
            else
            {
                MenuManager.instance.UnlockHuntersEyes(shopBtn.index, eyesPrice);
            }
        }
        ShowPanelUI();
    }
}
