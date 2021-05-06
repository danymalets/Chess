using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Advertisements;

public class MenuUI : MonoBehaviour
{
#if UNITY_ANDROID
    public const string GAME_ID = "4115773";
#elif UNITY_IOS
    public const string GAME_ID = "4115772";
#endif

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

    [SerializeField] private Animation _animation;

    private AsyncOperation _loading;

    public static bool IsGameLoaded = false;


    private void Start()
    {
        if (IsGameLoaded)
        {
            _animation.Play("MenuOpening");
        }
        else
        {
            IsGameLoaded = true;
            Advertisement.Initialize(GAME_ID, false);
            Application.targetFrameRate = 60;
        }

        _level.value = Prefs.LevelSlider;
        _moveDuration.value = Prefs.MoveDurationSlider;
        _roomNumber.text = Prefs.RoomName;
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

    public void OnLevelSliderValueChanged()
    {
        Prefs.LevelSlider = (int)_level.value;
    }

    public void OnTwoPlayersButtonClicked()
    {
        TimeSpan moveDuration = _moveDurations[(int)_moveDuration.value - 1];
        LoadChessGame(new TwoPlayers(moveDuration));
    }

    public void OnRoomNumberEditingEnd()
    {
        while (_roomNumber.text.Length < 3)
        {
            _roomNumber.text = "0" + _roomNumber.text;
        }
        Prefs.RoomName = _roomNumber.text;
    }

    public void OnNetworkFriendButtonClicked()
    {
        TimeSpan moveDuration = _moveDurations[(int)_moveDuration.value - 1];
        string roomName = _roomNumber.text;
        LoadChessGame(new NetworkFriend(moveDuration, roomName));
    }

    public void OnMoveDurationSliderValueChanged()
    {
        Prefs.MoveDurationSlider = (int)_moveDuration.value;
    }

    public void OnBotVsBotButtonClicked()
    {
        LoadChessGame(new BotVsBot((int)_level.value));
    }

    public void OnNetworkRandomRivalButtonClicked()
    {
        LoadChessGame(new NetworkRandomRival());
    }

    private void LoadChessGame(GameController controller)
    {
        GameController.MainController = controller;
        LoadScene("Game");
    }

    public void OnSettingsButtonClicked()
    {
        LoadScene("Settings");
    }

    public void OnHistoryButtonClicked()
    {
        LoadScene("SavedGames");
    }

    private void LoadScene(string scene)
    {
        _loading = SceneManager.LoadSceneAsync(scene);
        _loading.allowSceneActivation = false;
        _animation.Play("MenuClosing");
    }

    public void OnOpeningAnimationPlayed()
    {
        if (GameController.TotalMovesCount >= 10)
        {
            if (Advertisement.IsReady())
            {
                Advertisement.Show();
            }
            else
            {
                Debug.Log("add not ready");
            }
        }
        else
        {
            Debug.Log("total moves count = " + GameController.TotalMovesCount);
        }
    }

    public void OnClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }
}
