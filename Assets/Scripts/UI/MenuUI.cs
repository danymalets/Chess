using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Advertisements;
using UnityEngine.Serialization;

public class MenuUI : MonoBehaviour, IUI
{
    private static TimeSpan[] s_moveDurations = 
    {
        new TimeSpan(0, 0, 10),
        new TimeSpan(0, 0, 30),
        new TimeSpan(0, 1, 0),
        new TimeSpan(0, 2, 0),
        new TimeSpan(0, 5, 0),
        new TimeSpan(0, 10, 0)
    };
    [SerializeField] private Slider _levelSlider;
    [SerializeField] private Slider _moveDurationSlider;

    [SerializeField] private InputField _roomNumber;

    [SerializeField] private Animator _animator;

    private AsyncOperation _loading;

    public static bool IsGameLoaded = false;

    private void Start()
    {
        if (IsGameLoaded)
        {
            _animator.Play(UIAnimations.LeftOpening);
        }
        else
        {
            IsGameLoaded = true;
            Application.targetFrameRate = 60;
        }

        _levelSlider.value = Prefs.LevelSlider;
        _moveDurationSlider.value = Prefs.MoveDurationSlider;
        _roomNumber.text = Prefs.RoomName;
    }

    public void OnWhiteButtonClicked()
    {
        LoadChessGame(new SinglePlayer(Color.White, (int)_levelSlider.value));
    }

    public void OnBlackButtonClicked()
    {
        LoadChessGame(new SinglePlayer(Color.Black, (int)_levelSlider.value));
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
        Prefs.LevelSlider = (int)_levelSlider.value;
    }

    public void OnTwoPlayersButtonClicked()
    {
        TimeSpan moveDuration = s_moveDurations[(int)_moveDurationSlider.value - 1];
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
        TimeSpan moveDuration = s_moveDurations[(int)_moveDurationSlider.value - 1];
        string roomName = _roomNumber.text;
        LoadChessGame(new NetworkFriend(moveDuration, roomName));
    }

    public void OnMoveDurationSliderValueChanged()
    {
        Prefs.MoveDurationSlider = (int)_moveDurationSlider.value;
    }

    public void OnBotVsBotButtonClicked()
    {
        LoadChessGame(new BotVsBot((int)_levelSlider.value));
    }

    public void OnNetworkRandomRivalButtonClicked()
    {
        LoadChessGame(new NetworkRandomRival());
    }

    private void LoadChessGame(GameController controller)
    {
        SceneTransition.GameController = controller;
        LoadSceneAsync(Scenes.Game);
    }

    public void OnSettingsButtonClicked()
    {
        LoadSceneAsync(Scenes.Settings);
    }

    public void OnHistoryButtonClicked()
    {
        LoadSceneAsync(Scenes.SavedGames);
    }

    private void LoadSceneAsync(string scene)
    {
        _loading = SceneManager.LoadSceneAsync(scene);
        _loading.allowSceneActivation = false;
        _animator.Play(UIAnimations.LeftClosing);
    }

    public void OnOpeningAnimationPlayed()
    {
    }

    public void OnClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }
}
