using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavedGamePanel : MonoBehaviour
{
    [SerializeField] private BoardImage _board;

    [SerializeField] private Text _title;
    [SerializeField] private Text _info;
    [SerializeField] private Text _dateTime;

    public StoredGame StoredGame;

    public event Action<SavedGamePanel> DeleteButtonClicked;
    public event Action<StoredGame> RunButtonClicked;

    public void Init(StoredGame storedGame)
    {
        StoredGame = storedGame;

        _board.InitBoard();
        _board.InitPieces(new Game(storedGame.Moves).Position);

        _title.text = storedGame.GetTitle();
        _info.text = storedGame.GetInfo();
        _dateTime.text = storedGame.GetDateTime();
        SetRotation(storedGame.GetRotation());
    }

    private void SetRotation(int rotation)
    {
        _board.SetRotation(rotation);
    }

    public void OnDeleteButtonClicked()
    {
        DeleteButtonClicked?.Invoke(this);
    }

    public void OnRunButtonClicked()
    {
        RunButtonClicked?.Invoke(StoredGame);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
