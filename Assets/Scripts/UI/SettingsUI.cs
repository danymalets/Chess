﻿using System;
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

    private AsyncOperation _loading;

    private void Start()
    {
#if DEBUG
        if (!MenuUI.IsGameLoaded) { SceneManager.LoadScene(Scenes.MainMenu); return; }
#endif
        _animation.Play();
        _board.InitBoard();
        _board.InitPieces(new Position());
    }

    public void OnExit()
    {
        _loading = SceneManager.LoadSceneAsync(Scenes.MainMenu);
        _loading.allowSceneActivation = false;
        _animation.Play("SettingsClosing");
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
