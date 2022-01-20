using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePlayerAvatar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _avatarIcon;
    [SerializeField] private Transform _xImage;
    [SerializeField] private CanvasGroup _blackImg;

    private Health _currentHolderHealth;
    private bool _isDead;

    private void Update()
    {
        if (_currentHolderHealth == null) return;

            _slider.value = _currentHolderHealth.HP;

        if(_currentHolderHealth.isDead && _isDead == false)
        {
            _isDead = true;
            SetDead();
        }
    }

    public void SetAvatarIcon(Sprite icon)
    {
        _avatarIcon.sprite = icon;
    }

    public void SetHealthComponent(Health component)
    {
        _currentHolderHealth = component;
        if (_currentHolderHealth == null) return;
        _slider.maxValue = _currentHolderHealth.maxHealth;
        _slider.value = _currentHolderHealth.maxHealth;
        SetAvatarIcon(_currentHolderHealth.spriterenderer.sprite);
    }

    private void SetDead()
    {
        _xImage.gameObject.SetActive(true);
        _xImage.LeanScale(new Vector3(0.5f,0.5f ,1f), 0.5f);
        var imgColor = GetComponent<Image>();
        var tempColor = imgColor.color;
        tempColor.a = 0;
        imgColor.color = tempColor;
        _blackImg.LeanAlpha(1f, 0.5f);
    }

}
