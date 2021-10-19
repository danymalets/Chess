using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour, IView
{
    [SerializeField] private SpriteManager _spriteManager;

    [SerializeField] private Animator _animator;

    [SerializeField] private BoardImage _board;

    private AsyncOperation _loading;

    private void Start()
    {
#if DEBUG
        if (!MenuView.IsGameLoaded) { SceneManager.LoadScene(Scenes.MainMenu); return; }
#endif
        _board.InitBoard();
        _animator.Play(UIAnimations.RightOpening);
    }

    public void OnExit()
    {
        _loading = SceneManager.LoadSceneAsync(Scenes.MainMenu);
        _loading.allowSceneActivation = false;
        _animator.Play(UIAnimations.RightClosing);
    }

    public void OnOpeningAnimationPlayed()
    {
        _board.InitPieces(new Position());
    }

    public void OnClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }

    public void OnLeftButtonClicked()
    {
        _spriteManager.Prev();
        Redraw();
    }

    public void OnRightButtonClicked()
    {
        _spriteManager.Next();
        Redraw();
    }

    private void Redraw()
    {
        _board.Clear();
        _board.Draw();
        _board.InitPieces(new Position());
    }
}
