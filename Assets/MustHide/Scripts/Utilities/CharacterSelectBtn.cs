using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterSeletctBtnState
{
    CanSelect,
    CantSelect,
    IsSelected,
    NotPurshased,
    Diselect,
}

public class CharacterSelectBtn : MonoBehaviour
{
    [SerializeField] private CharacterSeletctBtnState _currentState;
    [SerializeField] private Image _currentCharacterImage;
    [SerializeField] private GameObject LockObject;
    [SerializeField] private GameObject MarkObject;
    [SerializeField] private CanvasGroup SelectedFadeCanvas;
    [SerializeField] private Sprite _normalImage;
    [SerializeField] private Sprite _blackAndWhiteImage;

    private Button _btn;

    public CharacterSeletctBtnState currentState => _currentState;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetState(CharacterSeletctBtnState state)
    {
        _currentState = state;
        OnEnterState();
    }

    private void OnEnterState()
    {
        if(_currentState == CharacterSeletctBtnState.NotPurshased)
        {
            LockObject.SetActive(true);
            _currentCharacterImage.sprite = _blackAndWhiteImage;
            GetComponent<Button>().interactable = false;
        }
        else if (_currentState == CharacterSeletctBtnState.CanSelect)
        {
            LockObject.SetActive(false);
            _currentCharacterImage.sprite = _normalImage;
            GetComponent<Button>().interactable = true;
        }
        else if (_currentState == CharacterSeletctBtnState.CantSelect)
        {
            LockObject.SetActive(false);
            _currentCharacterImage.sprite = _blackAndWhiteImage;
            GetComponent<Button>().interactable = false;
        }
        else if (_currentState == CharacterSeletctBtnState.IsSelected)
        {
            DoSelectAnimation();
            _currentCharacterImage.sprite = _normalImage;
            GetComponent<Button>().interactable = false;
        }
        else if (_currentState == CharacterSeletctBtnState.Diselect)
        {
            DoDiselectAnimation();
            GetComponent<Button>().interactable = true;
            SetState(CharacterSeletctBtnState.CanSelect);
        }
    }

    private void DoSelectAnimation()
    {
        transform.LeanScale(new Vector3(1.15f, 1.15f, 1.15f), 0.3f);
        SelectedFadeCanvas.LeanAlpha(1f, 0.3f);
    }

    private void DoDiselectAnimation()
    {
        transform.LeanScale(new Vector3(1f, 1f, 1f), 0.3f);
        SelectedFadeCanvas.LeanAlpha(0f, 0.3f);
    }

    public void EnableMark()
    {
        MarkObject.SetActive(true);
    }

    public void DisableMark()
    {
        MarkObject.SetActive(false);
    }


}
