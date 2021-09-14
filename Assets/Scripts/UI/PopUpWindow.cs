using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWindow : MonoBehaviour
{
    private const float Duration = 0.3f;
    
    [SerializeField] private Transform _window;
    private RawImage _tintedGlass;

    private void Awake()
    {
        _tintedGlass = GetComponent<RawImage>();
    }
    
    public void Open()
    {
        gameObject.SetActive(true);
        _window.localScale = Vector3.zero;
        _tintedGlass.color = new UnityEngine.Color(0.5f, 0.5f, 0.5f, 0);
        _tintedGlass.DOColor(new UnityEngine.Color(0.5f, 0.5f, 0.5f, 0.5f), Duration);
        _window.DOScale(1, Duration);
    }

    public void Close()
    {
        _window.localScale = Vector3.one;
        _tintedGlass.color = new UnityEngine.Color(0.5f, 0.5f, 0.5f, 0.5f);
        _tintedGlass.DOColor(new UnityEngine.Color(0.5f, 0.5f, 0.5f, 0), Duration);
        _window.DOScale(0, Duration)
            .OnComplete(() => gameObject.SetActive(false));
    }
}