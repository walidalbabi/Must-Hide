using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectBtn : MonoBehaviour
{

    [SerializeField] private Image _currentCharacterImage;
    [SerializeField] private Sprite _normalImage;
    [SerializeField] private Sprite _blackAndWhiteImage;

    private Button _btn;

    // Start is called before the first frame update
    void Start()
    {
        _btn = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_btn.interactable)
        {
            _currentCharacterImage.sprite = _normalImage;
        }
        else _currentCharacterImage.sprite = _blackAndWhiteImage;
    }
}
