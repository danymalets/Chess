using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MenuUI : MonoBehaviour
{
    private static TimeSpan[] _moveDurations = new TimeSpan[6]
    {
        new TimeSpan(0, 0, 10),
        new TimeSpan(0, 0, 30),
        new TimeSpan(0, 1, 0),
        new TimeSpan(0, 2, 0),
        new TimeSpan(0, 5, 0),
        new TimeSpan(0, 10, 0)
    };

    [SerializeField] private SpriteManager _spriteManager;

    [SerializeField] private Slider _level;
    [SerializeField] private Slider _moveDuration;

    [SerializeField] private InputField _roomNumber;

    [SerializeField] private Animation _closing;

    private AsyncOperation _loading;

    public static bool IsGameLoaded = false;

    void Start()
    {
        if (IsGameLoaded)
        {
            _closing.Play("MenuOpening");
        }
        else
        {
            IsGameLoaded = true;

            Application.targetFrameRate = 60;
        }
    }

    public void OnRoomNumberEditingEnd()
    {
        while (_roomNumber.text.Length < 3)
        {
            _roomNumber.text = "0" + _roomNumber.text;
        }
    }

    public void OnWhiteButtonClicked()
    {
        LoadChessGame(new SinglePlayer(Color.White, (int)_level.value));
    }

    public void OnBlackButtonClicked()
    {
        LoadChessGame(new SinglePlayer(Color.Black, (int)_level.value));
    }

    public void OnRandomColorButtonClicked()
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
        TimeSpan moveDuration = _moveDurations[(int)_moveDuration.value - 1];
        LoadChessGame(new TwoPlayers(moveDuration));
    }

    public void OnNetworkFriendButtonClicked()
    {
        TimeSpan moveDuration = _moveDurations[(int)_moveDuration.value - 1];
        string roomName = _roomNumber.text;
        LoadChessGame(new NetworkFriend(moveDuration, roomName));
    }

    public void OnNetworkRandomRivalButtonClicked()
    {
        LoadChessGame(new NetworkRandomRival());
    }

    public void OnBotVsBotButtonClicked()
    {
        LoadChessGame(new BotVsBot((int)_level.value));
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
        _loading = SceneManager.LoadSceneAsync("SavedGames");
        _loading.allowSceneActivation = false;
        _closing.Play("MenuClosing");
    }

    public void OnClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }
}
