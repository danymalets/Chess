using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Text _textTimer;
    [SerializeField] private Image _lineTimer;
    [SerializeField] private float _maxPing;
    
    [SerializeField] private UnityEngine.Color _userTimerColor;
    [SerializeField] private UnityEngine.Color _rivalTimerColor;
    
    public event Action UserTimeIsOver;
    public event Action RivalTimeIsOver;
    
    private Coroutine _countdown;

    public void CancelCountdown() => StopCoroutine(_countdown);
    
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
            yield return new WaitForSeconds(_maxPing);
            _textTimer.text = "время вышло";
            RivalTimeIsOver?.Invoke();
        }
    }

    public void SetTimeIsOver()
    {
        _textTimer.text = "время вышло";
    }
    
    
    public void Clear()
    {
        if (_countdown != null) StopCoroutine(_countdown);
        _textTimer.text = "00:00.00";
        _lineTimer.fillAmount = 0f;
    }
}