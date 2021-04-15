using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour
{
    [SerializeField] private SpriteManager _spriteManager;

    [SerializeField] private GameObject _background;

    [SerializeField] private Slider _slider;

    [SerializeField] private Animation _closing;

    private AsyncOperation _loading;

    public static bool IsGameLoaded = false;

    void Start()
    {
        Application.targetFrameRate = 60;
        if (IsGameLoaded)
        {
            _closing.Play("MenuOpening");
        }
        else
        {
            IsGameLoaded = true;
        }
    }

    public void OnWhiteButtonClicked()
    {
        LoadChessGame(new SinglePlayer(Color.White, (int)_slider.value));
    }

    public void OnBlackButtonClicked()
    {
        LoadChessGame(new SinglePlayer(Color.Black, (int)_slider.value));
    }

    public void OnRandomButtonClicked()
    {
        if (Random.Range(0, 2) == 0)
        {
            OnWhiteButtonClicked();
        }
        else
        {
            OnBlackButtonClicked();
        }
    }

    public void OnTwoPlayersButtonClicked()
    {
        LoadChessGame(new TwoPlayers(new TimeSpan(0, 2, 0)));
    }

    public void OnMultiPlayerButtonClicked()
    {
        LoadChessGame(new MultiPlayer(new TimeSpan(0, 2, 0)));
    }

    public void OnBotVsBotButtonClicked()
    {
        LoadChessGame(new BotVsBot((int)_slider.value));
    }

    private void LoadChessGame(GameController controller)
    {
        GameController.Singleton = controller;
        _loading = SceneManager.LoadSceneAsync("Game");
        _loading.allowSceneActivation = false;
        _closing.Play("MenuClosing");
    }

    public void OnHistoryButtonClicked()
    {
        _loading = SceneManager.LoadSceneAsync("Saved");
        _loading.allowSceneActivation = false;
        _closing.Play("MenuClosing");
    }

    public void OnClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }
}
