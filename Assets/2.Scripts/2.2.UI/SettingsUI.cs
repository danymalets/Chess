using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private SpriteManager _spriteManager;

    [SerializeField] private Animation _animation;

    [SerializeField] private BoardImage _board;

    [SerializeField] private FunctionButton _left;
    [SerializeField] private FunctionButton _right;

    private AsyncOperation _loading;

    private void Start()
    {
#if DEBUG
        if (!MenuUI.IsGameLoaded) { SceneManager.LoadScene("Menu"); return; }
#endif
        _animation.Play();
        _board.InitBoard();
        _board.InitPieces(new Position());
    }

    public void OnExit()
    {
        _loading = SceneManager.LoadSceneAsync("Menu");
        _loading.allowSceneActivation = false;
        _animation.Play("SettingsClosing");
    }

    public void OnClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }

    public void OnLeftButtomClicked()
    {
        if (_right.IsPressed) _spriteManager.SetEasterEgg();
        else _spriteManager.Prev();
        Redraw();
    }

    public void OnRightButtomClicked()
    {
        if (_left.IsPressed) _spriteManager.SetEasterEgg();
        else _spriteManager.Next();
        Redraw();
    }

    private void Redraw()
    {
        _board.Clear();
        _board.Draw();
        _board.InitPieces(new Position());
    }
}
