using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveFinder : MonoBehaviour
{
    [SerializeField] private Text _textTimer;
    [SerializeField] private Image _lineTimer;
    [SerializeField] private UnityEngine.Color _brainColor;

    public event Action<Move> MoveFound;

    public void StartSearchMove(AI ai) => StartCoroutine(SearchMove(ai));

    private IEnumerator SearchMove(AI ai)
    {
        _lineTimer.color = _brainColor;

        ai.StartSolving();

        DateTime startTime = DateTime.UtcNow;
        _textTimer.text = "00:00.00";
        _lineTimer.fillAmount = 0f;

        while (true)
        {
            yield return null;
            TimeSpan elapsedTime = DateTime.UtcNow - startTime;
            _textTimer.text = elapsedTime.TotalHours >= 1f ? "> 1 часа" : elapsedTime.ToString(@"mm\:ss\.ff");
            _lineTimer.fillAmount = ai.GetPart();
            if (ai.IsSolved(out Move move))
            {
                MoveFound?.Invoke(move);
                break;
            }
        }
    }
}