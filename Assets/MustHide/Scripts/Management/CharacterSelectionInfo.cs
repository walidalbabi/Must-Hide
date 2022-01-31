using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionInfo : MonoBehaviour
{

    [SerializeField] private Image _characterImage;
    [SerializeField] private Image _characterLogo;
    [SerializeField] private TextMeshProUGUI _characterNameTxt;
    [SerializeField] private TextMeshProUGUI _characterDescriptionTxt;
    

    public void SetCharacterSelectionInfo(CharacterInfoHolder info)
    {
        _characterImage.sprite = info._characterNormalImage;
        _characterLogo.sprite = info._characterNormalLogo;
        _characterNameTxt.text = info._characterName;
        _characterDescriptionTxt.text = info._characterDescription;
    }
}
