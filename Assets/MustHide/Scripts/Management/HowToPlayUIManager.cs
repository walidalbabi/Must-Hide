using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayUIManager : MonoBehaviour
{

    [SerializeField] private CanvasGroup[] _panelsCanvas;

    [SerializeField] private GameObject _nextBtn;
    [SerializeField] private GameObject _backBtn;

    private int _indexPage = 1;
    private CanvasGroup _currentSelectedPanel;


    private void Start()
    {
        _currentSelectedPanel = _panelsCanvas[0];
    }


    public void Next()
    {
        _indexPage++;
        _indexPage = Mathf.Clamp(_indexPage, 0, _panelsCanvas.Length);
        SetSelectedPane();
    }

    public void Back()
    {
        _indexPage--;
        _indexPage = Mathf.Clamp(_indexPage, 0, _panelsCanvas.Length);
        SetSelectedPane();
    }


    private void SetSelectedPane()
    {
        if(_currentSelectedPanel != null)
        {
            var previousSelected = _currentSelectedPanel;
            previousSelected.LeanAlpha(0f ,0.3f);
        }

        _currentSelectedPanel = _panelsCanvas[_indexPage - 1];
        _currentSelectedPanel.LeanAlpha(1f, 0.3f);

        SetButtonsState();
    }

    //Next and Back Button
    private void SetButtonsState()
    {
        if (!_backBtn.activeInHierarchy) _backBtn.SetActive(true);
        if (!_nextBtn.activeInHierarchy) _nextBtn.SetActive(true);

        if (_indexPage <= 1)
        {
            _backBtn.SetActive(false);
        }else if (_indexPage >= _panelsCanvas.Length)
        {
            _nextBtn.SetActive(false);
        }
    }
}
