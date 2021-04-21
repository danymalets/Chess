using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Line colors")]
    [SerializeField] private UnityEngine.Color _brainColor;
    [SerializeField] private UnityEngine.Color _userTimerColor;
    [SerializeField] private UnityEngine.Color _rivalTimerColor;

    [Header("Scene objects")]
    [SerializeField] private Board _board;
    [SerializeField] private Button _undo;
    [SerializeField] private Text _title;
    [SerializeField] private Text _status;
    [SerializeField] private Text _textTimer;
    [SerializeField] private Image _lineTimer;

    [Header("Network")]
    [SerializeField] private float _maxNetworkDelay;

    [Header("Animation")]
    [SerializeField] private Animation _animation;

    private Coroutine _countdown;

    private AsyncOperation _loading;

    public event Action<Move> MoveFound;
    public event Action UserTimeIsOver;
    public event Action RivalTimeIsOver;

    private GameController _controller;

    private AI _ai;

    private void Start()
    {
        _controller = GameController.Singleton;
#if DEBUG
        if (!MenuUI.IsGameLoaded) { SceneManager.LoadScene("Menu"); return; }
#endif
        if (!(_controller is SinglePlayer))
        {
            Destroy(_undo.gameObject);
        }
    }

    public void OnOpeningAnimationPlayed()
    {
        _controller.Init(this, _board);
    }

    public void SetTitle(string title)
    {
        _title.text = title;
    }

    public void SetStatus(string newStatus)
    {
        _status.text = newStatus;
    }

    public void StartSearchMove(AI ai) => StartCoroutine(SearchMove(ai));

    private IEnumerator SearchMove(AI ai)
    {
        _ai = ai;
        _lineTimer.color = _brainColor;

        _ai.StartSolving();

        DateTime startTime = DateTime.UtcNow;
        _textTimer.text = "00:00.00";
        _lineTimer.fillAmount = 0f;
        yield return null;

        while (true)
        {
            yield return null;
            TimeSpan elapsedTime = DateTime.UtcNow - startTime;
            if (elapsedTime.TotalHours >= 1)
            {
                _textTimer.text = "> 1 часа";
            }
            else
            {
                _textTimer.text = elapsedTime.ToString(@"mm\:ss\.ff");
            }
            _lineTimer.fillAmount = ai.GetPart();
            if (ai.IsSolved(out Move move))
            {
                MoveFound?.Invoke(move);
                break;
            }
        }
    }

    public NetworkRivalProvider GetNetworkProvider() => gameObject.AddComponent<NetworkRivalProvider>();

    public void StartUserTimer(TimeSpan duration) =>
        _countdown = StartCoroutine(Countdown(duration, true));

    public void StartRivalTimer(TimeSpan duration) =>
        _countdown = StartCoroutine(Countdown(duration, false));

    private IEnumerator Countdown(TimeSpan duration, bool isUserTimer)
    {
        _lineTimer.color = isUserTimer ? _userTimerColor : _rivalTimerColor;

        DateTime deadline = DateTime.UtcNow + duration;
        TimeSpan remainingTime = duration;
        while (remainingTime.TotalMilliseconds > 0)
        {
            _textTimer.text = remainingTime.ToString(@"mm\:ss\.ff");
            _lineTimer.fillAmount = (float)(remainingTime.TotalMilliseconds / duration.TotalMilliseconds);
            yield return null;
            remainingTime = deadline - DateTime.UtcNow;
        }

        _lineTimer.fillAmount = 0f;

        if (isUserTimer)
        {
            _textTimer.text = "время вышло";
            UserTimeIsOver?.Invoke();
        }
        else
        {
            _textTimer.text = "...";
            yield return new WaitForSeconds(_maxNetworkDelay * 2);
            _textTimer.text = "время вышло";
            RivalTimeIsOver?.Invoke();
        }
    }

    public void SetTimeIsOver()
    {
        _textTimer.text = "время вышло";
    }

    public void CanselCoundown() => StopCoroutine(_countdown);

    public void OnUndoButtonClicked()
    {
        if (GameController.Singleton is SinglePlayer singlePlayer)
        {
            singlePlayer.Undo();
        }
    }

    public void Clear()
    {
        if (_ai != null)
        {
            _ai.Abort();
        }
        StopAllCoroutines();
        _textTimer.text = "00:00.00";
        _lineTimer.fillAmount = 0f;
    }

    public void OnExit()
    {
        if (_ai != null)
        {
            _ai.Abort();
        }
        _controller.Finish();
        _loading = SceneManager.LoadSceneAsync("Menu");
        _animation.Play("GameClosing");
        _loading.allowSceneActivation = false;
    }

    public void OnClosingAnimationPlayed()
    {
        _loading.allowSceneActivation = true;
    }

    public void OnApplicationQuit()
    {
        if (_ai != null)
        {
            _ai.Abort();
        }
        if (_loading == null)
        {
            _controller.Finish();
        }
    }
}
